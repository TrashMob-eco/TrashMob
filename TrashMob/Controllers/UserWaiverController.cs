namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Controller for user waiver operations.
    /// Handles waiver signing, viewing, and downloading.
    /// </summary>
    [Route("api/waivers")]
    public class UserWaiverController : SecureController
    {
        private readonly IUserWaiverManager userWaiverManager;
        private readonly IWaiverDocumentManager waiverDocumentManager;
        private readonly IUserManager userManager;
        private readonly IEventAttendeeManager eventAttendeeManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWaiverController"/> class.
        /// </summary>
        /// <param name="userWaiverManager">The user waiver manager.</param>
        /// <param name="waiverDocumentManager">The waiver document manager.</param>
        /// <param name="userManager">The user manager.</param>
        /// <param name="eventAttendeeManager">The event attendee manager.</param>
        public UserWaiverController(
            IUserWaiverManager userWaiverManager,
            IWaiverDocumentManager waiverDocumentManager,
            IUserManager userManager,
            IEventAttendeeManager eventAttendeeManager)
        {
            this.userWaiverManager = userWaiverManager;
            this.waiverDocumentManager = waiverDocumentManager;
            this.userManager = userManager;
            this.eventAttendeeManager = eventAttendeeManager;
        }

        /// <summary>
        /// Gets waivers the current user needs to sign.
        /// </summary>
        /// <param name="communityId">Optional community ID to include community-specific waivers.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of waiver versions requiring signature.</returns>
        [HttpGet("required")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequiredWaivers(
            [FromQuery] Guid? communityId = null,
            CancellationToken cancellationToken = default)
        {
            var result = await userWaiverManager
                .GetPendingWaiversForUserAsync(UserId, communityId, cancellationToken);
            TrackEvent(nameof(GetRequiredWaivers));

            return Ok(result);
        }

        /// <summary>
        /// Gets waivers required for a specific event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of waiver versions requiring signature for the event.</returns>
        [HttpGet("required/event/{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetRequiredWaiversForEvent(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var result = await userWaiverManager
                .GetRequiredWaiversForEventAsync(UserId, eventId, cancellationToken);
            TrackEvent(nameof(GetRequiredWaiversForEvent));

            return Ok(result);
        }

        /// <summary>
        /// Gets all waivers signed by the current user.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of user waivers.</returns>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<UserWaiver>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyWaivers(CancellationToken cancellationToken = default)
        {
            var result = await userWaiverManager
                .GetUserWaiversAsync(UserId, cancellationToken);
            TrackEvent(nameof(GetMyWaivers));

            return Ok(result);
        }

        /// <summary>
        /// Accepts a waiver and creates a signed record.
        /// Captures IP address and user agent for audit purposes.
        /// </summary>
        /// <param name="request">The waiver acceptance request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created user waiver.</returns>
        [HttpPost("accept")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserWaiver), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AcceptWaiver(
            [FromBody] AcceptWaiverApiRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return BadRequest("Request is required.");
            }

            if (request.WaiverVersionId == Guid.Empty)
            {
                return BadRequest("Waiver version ID is required.");
            }

            if (string.IsNullOrWhiteSpace(request.TypedLegalName))
            {
                return BadRequest("Typed legal name is required.");
            }

            // Capture IP address and user agent
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

            // Generate and store PDF
            var userWaiver = result.Data;
            var user = await userManager.GetAsync(UserId, cancellationToken);

            try
            {
                // Get the full waiver with version details
                var waiverWithDetails = await userWaiverManager
                    .GetUserWaiverWithDetailsAsync(userWaiver.Id, cancellationToken);

                var documentUrl = await waiverDocumentManager
                    .GenerateAndStoreWaiverPdfAsync(waiverWithDetails, user, cancellationToken);

                // Update the user waiver with the document URL
                userWaiver.DocumentUrl = documentUrl;
                await userWaiverManager.UpdateAsync(userWaiver, cancellationToken);
            }
            catch (Exception ex)
            {
                // Don't fail the waiver acceptance if PDF generation fails
                // The waiver is still valid, PDF can be regenerated later
                Logger?.LogWarning(ex, "PDF generation failed for waiver {WaiverId}", userWaiver.Id);
            }

            TrackEvent(nameof(AcceptWaiver));

            return CreatedAtAction(nameof(GetMyWaivers), userWaiver);
        }

        /// <summary>
        /// Gets a specific user waiver by ID.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The user waiver details.</returns>
        [HttpGet("{userWaiverId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserWaiver), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetUserWaiver(
            Guid userWaiverId,
            CancellationToken cancellationToken = default)
        {
            var waiver = await userWaiverManager
                .GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken);

            if (waiver == null)
            {
                return NotFound();
            }

            // Only allow users to see their own waivers
            if (waiver.UserId != UserId)
            {
                return Forbid();
            }

            TrackEvent(nameof(GetUserWaiver));

            return Ok(waiver);
        }

        /// <summary>
        /// Downloads the PDF for a signed waiver.
        /// </summary>
        /// <param name="userWaiverId">The user waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The waiver PDF file.</returns>
        [HttpGet("{userWaiverId}/download")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> DownloadWaiverPdf(
            Guid userWaiverId,
            CancellationToken cancellationToken = default)
        {
            var waiver = await userWaiverManager
                .GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken);

            if (waiver == null)
            {
                return NotFound();
            }

            // Only allow users to download their own waivers
            if (waiver.UserId != UserId)
            {
                return Forbid();
            }

            // If no stored document, generate one on-the-fly
            if (string.IsNullOrWhiteSpace(waiver.DocumentUrl))
            {
                var user = await userManager.GetAsync(waiver.UserId, cancellationToken);
                var pdfBytes = waiverDocumentManager.GenerateWaiverPdf(waiver, user);

                TrackEvent(nameof(DownloadWaiverPdf));

                return File(pdfBytes, "application/pdf", $"waiver-{waiver.Id}.pdf");
            }

            TrackEvent(nameof(DownloadWaiverPdf));

            // Redirect to the blob URL
            return Redirect(waiver.DocumentUrl);
        }

        /// <summary>
        /// Checks if the current user has valid waivers for an event.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>True if user has all required waivers signed.</returns>
        [HttpGet("check/event/{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverCheckResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckWaiversForEvent(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
            var hasValidWaiver = await userWaiverManager
                .HasValidWaiverForEventAsync(UserId, eventId, cancellationToken);

            TrackEvent(nameof(CheckWaiversForEvent));

            return Ok(new WaiverCheckResult { HasValidWaiver = hasValidWaiver });
        }

        /// <summary>
        /// Uploads a paper waiver on behalf of a user.
        /// Can be used by event leads for their event attendees, or by admins for any user.
        /// </summary>
        /// <param name="request">The paper waiver upload request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created user waiver.</returns>
        [HttpPost("upload-paper")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(UserWaiver), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UploadPaperWaiver(
            [FromForm] PaperWaiverUploadApiRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null)
            {
                return BadRequest("Request is required.");
            }

            if (request.FormFile == null || request.FormFile.Length == 0)
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

            TrackEvent(nameof(UploadPaperWaiver));

            return CreatedAtAction(nameof(GetUserWaiver), new { userWaiverId = result.Data.Id }, result.Data);
        }

        /// <summary>
        /// Gets waiver status for all attendees of an event.
        /// Only accessible by event leads.
        /// </summary>
        /// <param name="eventId">The event ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of attendee waiver statuses.</returns>
        [HttpGet("event/{eventId}/attendees")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<AttendeeWaiverStatus>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetEventAttendeeWaiverStatus(
            Guid eventId,
            CancellationToken cancellationToken = default)
        {
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

            TrackEvent(nameof(GetEventAttendeeWaiverStatus));

            return Ok(result);
        }

        private string GetClientIpAddress()
        {
            // Check for X-Forwarded-For header (when behind a proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrWhiteSpace(forwardedFor))
            {
                // X-Forwarded-For can contain multiple IPs, take the first one
                return forwardedFor.Split(',')[0].Trim();
            }

            // Fall back to direct connection IP
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }
    }

    /// <summary>
    /// API request model for accepting a waiver.
    /// </summary>
    public class AcceptWaiverApiRequest
    {
        /// <summary>
        /// Gets or sets the waiver version being accepted.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name entered by the signer.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets whether the signer is a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's user ID if the signer is a minor.
        /// </summary>
        public Guid? GuardianUserId { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name if not a registered user.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }
    }

    /// <summary>
    /// Result model for waiver check operations.
    /// </summary>
    public class WaiverCheckResult
    {
        /// <summary>
        /// Gets or sets whether the user has valid waivers.
        /// </summary>
        public bool HasValidWaiver { get; set; }
    }

    /// <summary>
    /// API request model for uploading a paper waiver.
    /// </summary>
    public class PaperWaiverUploadApiRequest
    {
        /// <summary>
        /// Gets or sets the uploaded file (PDF, JPEG, PNG, or WebP).
        /// </summary>
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the person who signed the waiver.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the waiver version being signed.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the name as written on the paper waiver.
        /// </summary>
        public string SignerName { get; set; }

        /// <summary>
        /// Gets or sets the date the paper waiver was signed.
        /// </summary>
        public DateTimeOffset DateSigned { get; set; }

        /// <summary>
        /// Gets or sets the optional event ID if uploading for a specific event.
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        /// Gets or sets whether the signer is a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name if the signer is a minor.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }
    }
}
