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
        ILogger<WaiversV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

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
