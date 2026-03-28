namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for processing PRIVO consent webhook events.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/webhooks/privo")]
    [TypeFilter(typeof(PrivoApiKeyAuthenticationFilter))]
    public class PrivoWebhooksV2Controller(
        IPrivoConsentManager privoConsentManager,
        ILogger<PrivoWebhooksV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Receives consent status updates from PRIVO.
        /// </summary>
        /// <param name="payload">The PRIVO webhook payload.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>200 OK on successful receipt.</returns>
        /// <response code="200">Consent event received successfully.</response>
        /// <response code="401">API key authentication failed.</response>
        [HttpPost("consent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ProcessConsentEvent(
            [FromBody] PrivoWebhookPayload payload, CancellationToken cancellationToken = default)
        {
            logger.LogInformation(
                "V2 Received PRIVO webhook: Id={WebhookId}, EventTypes={EventTypes}, Sid={Sid}",
                payload.Id,
                string.Join(",", payload.EventTypes),
                payload.Sid);

            try
            {
                await privoConsentManager.ProcessWebhookAsync(payload, cancellationToken);
            }
            catch (Exception ex)
            {
                // Always return 200 to PRIVO — log the error but don't reject the webhook
                logger.LogError(ex, "Error processing PRIVO webhook {WebhookId}", payload.Id);
            }

            return Ok();
        }
    }
}
