namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for PRIVO consent management and identity verification.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/privo")]
    public class PrivoConsentV2Controller(
        IPrivoConsentManager privoConsentManager,
        IConfiguration configuration,
        ILogger<PrivoConsentV2Controller> logger) : ControllerBase
    {
        private bool IsPrivoEnabled => configuration.GetValue("Privo:Enabled", false);
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId)
            ? parsedUserId
            : Guid.Empty;

        /// <summary>
        /// Returns whether the PRIVO integration is enabled.
        /// </summary>
        /// <response code="200">Returns enabled status.</response>
        [HttpGet("enabled")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
        public IActionResult GetEnabled()
        {
            return Ok(new { enabled = IsPrivoEnabled });
        }

        /// <summary>
        /// Initiates adult identity verification via PRIVO (Flow 1).
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The consent record with PRIVO redirect URL.</returns>
        /// <response code="200">Verification initiated, redirect to ConsentUrl.</response>
        /// <response code="400">User already verified or validation error.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="503">PRIVO integration not enabled.</response>
        [HttpPost("verify")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ParentalConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitiateAdultVerification(CancellationToken cancellationToken)
        {
            if (!IsPrivoEnabled) return StatusCode(503, new { message = "PRIVO integration is not yet enabled." });

            logger.LogInformation("V2 InitiateAdultVerification for User={UserId}", UserId);

            var consent = await privoConsentManager.InitiateAdultVerificationAsync(UserId, cancellationToken);
            return Ok(consent.ToV2Dto());
        }

        /// <summary>
        /// Initiates parent-to-child consent for a 13-17 dependent via PRIVO (Flow 2).
        /// </summary>
        /// <param name="dependentId">The dependent to request consent for.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The consent record with PRIVO redirect URL.</returns>
        /// <response code="200">Consent request initiated, redirect to ConsentUrl.</response>
        /// <response code="400">Validation error (parent not verified, dependent age, etc.).</response>
        /// <response code="401">Not authenticated.</response>
        [HttpPost("consent/child/{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ParentalConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitiateParentChildConsent(Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 InitiateParentChildConsent for User={UserId}, Dependent={DependentId}",
                UserId, dependentId);

            var consent = await privoConsentManager.InitiateParentChildConsentAsync(UserId, dependentId, cancellationToken);
            return Ok(consent.ToV2Dto());
        }

        /// <summary>
        /// Initiates a child-driven consent request (Flow 3). No authentication required.
        /// </summary>
        /// <param name="request">The child's information and parent email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The consent record, or 204 if parent must create account first.</returns>
        /// <response code="200">Consent request created, PRIVO will email parent.</response>
        /// <response code="204">Parent account not found — parent must create account first.</response>
        /// <response code="400">Validation error (age, parent not verified, etc.).</response>
        [HttpPost("consent/child-initiated")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(ParentalConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> InitiateChildConsent(
            [FromBody] InitiateChildConsentRequest request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 InitiateChildConsent for ParentEmail={ParentEmail}", request.ParentEmail);

            var consent = await privoConsentManager.InitiateChildConsentAsync(
                request.ChildFirstName, request.ChildEmail, request.ChildBirthDate,
                request.ParentEmail, cancellationToken);

            if (consent == null)
            {
                // Parent account doesn't exist — NO FLOW
                return NoContent();
            }

            return Ok(consent.ToV2Dto());
        }

        /// <summary>
        /// Gets the current user's verification/consent status.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The most recent consent record, or 204 if none.</returns>
        /// <response code="200">Consent record found.</response>
        /// <response code="204">No consent record for this user.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpGet("status")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(ParentalConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> GetVerificationStatus(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetVerificationStatus for User={UserId}", UserId);

            var consent = await privoConsentManager.GetConsentByUserIdAsync(UserId, cancellationToken);
            if (consent == null)
            {
                return NoContent();
            }

            return Ok(consent.ToV2Dto());
        }

        /// <summary>
        /// Polls PRIVO for current consent status and updates the local record.
        /// Use when webhook delivery is delayed or not configured.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated consent record.</returns>
        /// <response code="200">Status refreshed.</response>
        /// <response code="204">No pending consent record for this user.</response>
        /// <response code="401">Not authenticated.</response>
        [HttpPost("status/refresh")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(ParentalConsentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RefreshVerificationStatus(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RefreshVerificationStatus for User={UserId}", UserId);

            var consent = await privoConsentManager.RefreshConsentStatusAsync(UserId, cancellationToken);
            if (consent == null)
            {
                return NoContent();
            }

            return Ok(consent.ToV2Dto());
        }

        /// <summary>
        /// Revokes consent for a specific consent record.
        /// </summary>
        /// <param name="consentId">The consent record to revoke.</param>
        /// <param name="reason">The reason for revocation.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>204 No Content on success.</returns>
        /// <response code="204">Consent revoked successfully.</response>
        /// <response code="400">Consent not found or validation error.</response>
        /// <response code="401">Not authenticated.</response>
        /// <response code="403">Not authorized to revoke this consent.</response>
        [HttpPost("consent/{consentId}/revoke")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RevokeConsent(Guid consentId, [FromQuery] string reason, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RevokeConsent ConsentId={ConsentId} by User={UserId}", consentId, UserId);

            await privoConsentManager.RevokeConsentAsync(consentId, UserId, reason, cancellationToken);
            return NoContent();
        }
    }
}
