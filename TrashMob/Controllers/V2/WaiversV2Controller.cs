namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// V2 controller for user waiver operations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/waivers")]
    public class WaiversV2Controller(
        IUserWaiverManager userWaiverManager,
        IWaiverDocumentManager waiverDocumentManager,
        IUserManager userManager,
        IWaiverManager waiverManager,
        IEventAttendeeManager eventAttendeeManager,
        ILogger<WaiversV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets waivers the current user needs to sign.
        /// </summary>
        /// <param name="communityId">Optional community ID to include community-specific waivers.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the required waiver versions.</response>
        [HttpGet("required")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<WaiverVersionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequired(
            [FromQuery] Guid? communityId = null,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetRequiredWaivers CommunityId={CommunityId}", communityId);

            var result = await userWaiverManager.GetPendingWaiversForUserAsync(UserId, communityId, cancellationToken);
            var dtos = result.Select(w => w.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets all waivers signed by the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's signed waivers.</response>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<UserWaiverDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWaivers(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetMyWaivers");

            var result = await userWaiverManager.GetUserWaiversAsync(UserId, cancellationToken);
            var dtos = result.Select(w => w.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Accepts a waiver and creates a signed record.
        /// </summary>
        /// <param name="request">The waiver acceptance request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Waiver accepted.</response>
        /// <response code="400">Validation error.</response>
        [HttpPost("accept")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserWaiverDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Accept(
            [FromBody] AcceptWaiverRequestDto request,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 AcceptWaiver WaiverVersion={WaiverVersionId}", request.WaiverVersionId);

            // Minors cannot sign their own waivers — parent/guardian must sign DependentWaivers
            var currentUser = await userManager.GetAsync(UserId, cancellationToken);
            if (currentUser is { IsMinor: true })
            {
                return BadRequest("Minors cannot sign waivers. Your parent or guardian must sign waivers on your behalf from their dashboard.");
            }

            if (request.WaiverVersionId == Guid.Empty)
            {
                return BadRequest("Waiver version ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.TypedLegalName))
            {
                return BadRequest("Typed legal name is required.");
            }

            var ipAddress = GetClientIpAddress();
            var userAgent = Request.Headers.UserAgent.ToString();

            var acceptRequest = new AcceptWaiverRequest
            {
                WaiverVersionId = request.WaiverVersionId,
                TypedLegalName = request.TypedLegalName,
                UserId = UserId,
                IPAddress = ipAddress,
                UserAgent = userAgent,
                IsMinor = request.IsMinor,
                GuardianUserId = request.GuardianUserId,
                GuardianName = request.GuardianName,
                GuardianRelationship = request.GuardianRelationship,
            };

            var result = await userWaiverManager.AcceptWaiverAsync(acceptRequest, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            var userWaiver = result.Data;
            var user = await userManager.GetAsync(UserId, cancellationToken);

            try
            {
                var waiverWithDetails = await userWaiverManager
                    .GetUserWaiverWithDetailsAsync(userWaiver.Id, cancellationToken);

                var documentUrl = await waiverDocumentManager
                    .GenerateAndStoreWaiverPdfAsync(waiverWithDetails, user, cancellationToken);

                userWaiver.DocumentUrl = documentUrl;
                await userWaiverManager.UpdateAsync(userWaiver, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "PDF generation failed for waiver {WaiverId}", userWaiver.Id);
            }

            return CreatedAtAction(nameof(GetMyWaivers), userWaiver.ToV2Dto());
        }

        /// <summary>
        /// Gets waivers required for a specific event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the waiver versions required for the event.</response>
        [HttpGet("required/event/{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<WaiverVersionDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequiredWaiversForEvent(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetRequiredWaiversForEvent EventId={EventId}", eventId);

            var result = await userWaiverManager
                .GetRequiredWaiversForEventAsync(UserId, eventId, cancellationToken);
            var dtos = result.Select(w => w.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a specific user waiver by ID.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user waiver details.</response>
        /// <response code="403">User does not own this waiver.</response>
        /// <response code="404">Waiver not found.</response>
        [HttpGet("{userWaiverId:guid}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserWaiverDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUserWaiver(
            Guid userWaiverId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetUserWaiver UserWaiverId={UserWaiverId}", userWaiverId);

            var waiver = await userWaiverManager
                .GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken);

            if (waiver is null)
            {
                return NotFound();
            }

            // Only allow users to see their own waivers
            if (waiver.UserId != UserId)
            {
                return Forbid();
            }

            return Ok(waiver.ToV2Dto());
        }

        /// <summary>
        /// Checks if the current user has valid waivers for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns whether the user has valid waivers.</response>
        [HttpGet("check/event/{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverCheckResultDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckWaiversForEvent(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 CheckWaiversForEvent EventId={EventId}", eventId);

            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(UserId, eventId, cancellationToken);

            return Ok(new WaiverCheckResultDto { HasValidWaiver = hasValidWaiver });
        }

        /// <summary>
        /// Uploads a paper waiver on behalf of a user.
        /// Can be used by event leads for their event attendees, or by admins for any user.
        /// </summary>
        /// <param name="request">The paper waiver upload request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Paper waiver uploaded successfully.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not authorized to upload paper waivers.</response>
        [HttpPost("upload-paper")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserWaiverDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadPaperWaiver(
            [FromForm] PaperWaiverUploadApiRequest request,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 UploadPaperWaiver");

            if (request is null)
            {
                return BadRequest("Request is required.");
            }

            if (request.FormFile is null || request.FormFile.Length == 0)
            {
                return BadRequest("File is required.");
            }

            if (request.UserId == Guid.Empty)
            {
                return BadRequest("User ID is required.");
            }

            if (request.WaiverVersionId == Guid.Empty)
            {
                return BadRequest("Waiver version ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.SignerName))
            {
                return BadRequest("Signer name is required.");
            }

            // Check authorization
            var isAdmin = User.IsInRole("Admin");
            var isEventLead = false;

            if (request.EventId.HasValue)
            {
                isEventLead = await eventAttendeeManager
                    .IsEventLeadAsync(request.EventId.Value, UserId, cancellationToken);
            }

            // Event leads can only upload for their events, admins can upload for anyone
            if (!isAdmin && !isEventLead)
            {
                return Forbid();
            }

            var uploadRequest = new PaperWaiverUploadRequest
            {
                FormFile = request.FormFile,
                UserId = request.UserId,
                WaiverVersionId = request.WaiverVersionId,
                SignerName = request.SignerName,
                DateSigned = request.DateSigned,
                EventId = request.EventId,
                IsMinor = request.IsMinor,
                GuardianName = request.GuardianName,
                GuardianRelationship = request.GuardianRelationship,
            };

            var result = await userWaiverManager
                .UploadPaperWaiverAsync(uploadRequest, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetUserWaiver), new { userWaiverId = result.Data.Id }, result.Data.ToV2Dto());
        }

        /// <summary>
        /// Gets waiver status for all attendees of an event.
        /// Only accessible by event leads or admins.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the list of attendee waiver statuses.</response>
        /// <response code="403">User is not an event lead or admin.</response>
        [HttpGet("event/{eventId}/attendees")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<AttendeeWaiverStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEventAttendeeWaiverStatus(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetEventAttendeeWaiverStatus EventId={EventId}", eventId);

            // Check if user is event lead or admin
            var isAdmin = User.IsInRole("Admin");
            var isEventLead = await eventAttendeeManager
                .IsEventLeadAsync(eventId, UserId, cancellationToken);

            if (!isAdmin && !isEventLead)
            {
                return Forbid();
            }

            var result = await userWaiverManager
                .GetEventAttendeeWaiverStatusAsync(eventId, cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a waiver by name.
        /// </summary>
        /// <param name="name">The name of the waiver.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the waiver.</response>
        /// <response code="404">Waiver not found.</response>
        [HttpGet("by-name/{name}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetWaiverByName(
            string name,
            CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetWaiverByName Name={Name}", name);

            var waiver = await waiverManager.GetByNameAsync(name, cancellationToken);

            if (waiver is null)
            {
                return NotFound();
            }

            return Ok(waiver.ToV2Dto());
        }

        private string GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }

            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }
}
