namespace TrashMob.Controllers
{
    using System.Threading;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;
    using TrashMob.Security;

    /// <summary>
    /// Controller for processing PRIVO consent webhook events.
    /// </summary>
    [Route("api/webhooks/privo")]
    [ApiController]
    [TypeFilter(typeof(PrivoApiKeyAuthenticationFilter))]
    public class PrivoWebhooksController(
        ILogger<PrivoWebhooksController> logger) : ControllerBase
    {
        /// <summary>
        /// Receives consent status updates from PRIVO.
        /// </summary>
        /// <param name="consentEvent">The consent event payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>200 OK on successful receipt.</returns>
        [HttpPost("consent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public IActionResult ProcessConsentEvent([FromBody] PrivoConsentEvent consentEvent, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
                "Received PRIVO consent event: Type={EventType}, ConsentRequestId={ConsentRequestId}, Status={Status}",
                consentEvent.EventType,
                consentEvent.ConsentRequestId,
                consentEvent.Status);

            // TODO: Process consent status update when PRIVO payload spec is finalized
            // Will update DependentInvitation status based on consent result

            return Ok();
        }
    }
}
