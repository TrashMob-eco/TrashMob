namespace TrashMob.Controllers.V2
{
    using System.Threading;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;
    using TrashMob.Security;

    /// <summary>
    /// V2 controller for processing PRIVO consent webhook events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/webhooks/privo")]
    [TypeFilter(typeof(PrivoApiKeyAuthenticationFilter))]
    public class PrivoWebhooksV2Controller(
        ILogger<PrivoWebhooksV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Receives consent status updates from PRIVO.
        /// </summary>
        /// <param name="consentEvent">The consent event payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>200 OK on successful receipt.</returns>
        /// <response code="200">Consent event received successfully.</response>
        /// <response code="401">API key authentication failed.</response>
        [HttpPost("consent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ProcessConsentEvent([FromBody] PrivoConsentEvent consentEvent, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
                "V2 Received PRIVO consent event: Type={EventType}, ConsentRequestId={ConsentRequestId}, Status={Status}",
                consentEvent.EventType,
                consentEvent.ConsentRequestId,
                consentEvent.Status);

            // TODO: Process consent status update when PRIVO payload spec is finalized
            // Will update DependentInvitation status based on consent result

            return Ok();
        }
    }
}
