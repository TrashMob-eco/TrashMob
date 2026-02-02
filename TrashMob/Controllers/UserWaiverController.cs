namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
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

        /// <summary>
        /// Initializes a new instance of the <see cref="UserWaiverController"/> class.
        /// </summary>
        /// <param name="userWaiverManager">The user waiver manager.</param>
        /// <param name="waiverDocumentManager">The waiver document manager.</param>
        /// <param name="userManager">The user manager.</param>
        public UserWaiverController(
            IUserWaiverManager userWaiverManager,
            IWaiverDocumentManager waiverDocumentManager,
            IUserManager userManager)
        {
            this.userWaiverManager = userWaiverManager;
            this.waiverDocumentManager = waiverDocumentManager;
            this.userManager = userManager;
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
                .GetPendingWaiversForUserAsync(UserId, communityId, cancellationToken)
                .ConfigureAwait(false);
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
                .GetRequiredWaiversForEventAsync(UserId, eventId, cancellationToken)
                .ConfigureAwait(false);
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
                .GetUserWaiversAsync(UserId, cancellationToken)
                .ConfigureAwait(false);
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

            var result = await userWaiverManager.AcceptWaiverAsync(acceptRequest, cancellationToken).ConfigureAwait(false);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            // Generate and store PDF
            var userWaiver = result.Data;
            var user = await userManager.GetAsync(UserId, cancellationToken).ConfigureAwait(false);

            try
            {
                // Get the full waiver with version details
                var waiverWithDetails = await userWaiverManager
                    .GetUserWaiverWithDetailsAsync(userWaiver.Id, cancellationToken)
                    .ConfigureAwait(false);

                var documentUrl = await waiverDocumentManager
                    .GenerateAndStoreWaiverPdfAsync(waiverWithDetails, user, cancellationToken)
                    .ConfigureAwait(false);

                // Update the user waiver with the document URL
                userWaiver.DocumentUrl = documentUrl;
                await userWaiverManager.UpdateAsync(userWaiver, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception)
            {
                // Log but don't fail the waiver acceptance if PDF generation fails
                // The waiver is still valid, PDF can be regenerated later
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
                .GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken)
                .ConfigureAwait(false);

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
                .GetUserWaiverWithDetailsAsync(userWaiverId, cancellationToken)
                .ConfigureAwait(false);

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
                var user = await userManager.GetAsync(waiver.UserId, cancellationToken).ConfigureAwait(false);
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
                .HasValidWaiverForEventAsync(UserId, eventId, cancellationToken)
                .ConfigureAwait(false);

            TrackEvent(nameof(CheckWaiversForEvent));

            return Ok(new WaiverCheckResult { HasValidWaiver = hasValidWaiver });
        }

        private string GetClientIpAddress()
        {
            // Check for X-Forwarded-For header (when behind a proxy/load balancer)
            var forwardedFor = Request.Headers["X-Forwarded-For"].ToString();
            if (!string.IsNullOrEmpty(forwardedFor))
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
}
