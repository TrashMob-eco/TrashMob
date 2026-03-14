namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Controllers;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for processing SendGrid webhook events for newsletter tracking.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/webhooks/sendgrid")]
    public class NewsletterWebhooksV2Controller(
        INewsletterManager newsletterManager,
        ILogger<NewsletterWebhooksV2Controller> logger) : ControllerBase
    {
        /// <summary>
        /// Processes SendGrid webhook events for newsletter tracking.
        /// </summary>
        /// <param name="events">The list of SendGrid events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>OK status regardless of processing outcome.</returns>
        [HttpPost("newsletter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessNewsletterEvents([FromBody] List<SendGridEvent> events, CancellationToken cancellationToken = default)
        {
            if (events == null || !events.Any())
            {
                return Ok();
            }

            logger.LogInformation("V2 Received {Count} SendGrid events", events.Count);

            var eventsByNewsletter = events
                .Where(e => e.NewsletterIdValue.HasValue)
                .GroupBy(e => e.NewsletterIdValue.Value);

            foreach (var group in eventsByNewsletter)
            {
                var newsletterId = group.Key;
                var deliveredCount = group.Count(e => e.Event == "delivered");
                var openCount = group.Count(e => e.Event == "open");
                var clickCount = group.Count(e => e.Event == "click");
                var bounceCount = group.Count(e => e.Event == "bounce" || e.Event == "dropped");
                var unsubscribeCount = group.Count(e => e.Event == "unsubscribe" || e.Event == "group_unsubscribe");

                try
                {
                    await newsletterManager.UpdateStatisticsAsync(
                        newsletterId,
                        deliveredCount,
                        openCount,
                        clickCount,
                        bounceCount,
                        unsubscribeCount,
                        cancellationToken);

                    logger.LogInformation(
                        "V2 Updated newsletter {NewsletterId} stats: delivered={Delivered}, opened={Opened}, clicked={Clicked}, bounced={Bounced}, unsubscribed={Unsubscribed}",
                        newsletterId, deliveredCount, openCount, clickCount, bounceCount, unsubscribeCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "V2 Failed to update statistics for newsletter {NewsletterId}", newsletterId);
                }
            }

            return Ok();
        }
    }
}
