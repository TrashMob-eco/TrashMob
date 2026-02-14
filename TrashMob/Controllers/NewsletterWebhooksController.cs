namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for processing SendGrid webhook events for newsletter tracking.
    /// </summary>
    [Route("api/webhooks/sendgrid")]
    [ApiController]
    public class NewsletterWebhooksController(
        INewsletterManager newsletterManager,
        ILogger<NewsletterWebhooksController> logger)
        : ControllerBase
    {

        /// <summary>
        /// Processes SendGrid webhook events for newsletter tracking.
        /// </summary>
        /// <param name="events">The list of SendGrid events.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Success status.</returns>
        [HttpPost("newsletter")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> ProcessNewsletterEvents([FromBody] List<SendGridEvent> events, CancellationToken cancellationToken = default)
        {
            if (events is null || !events.Any())
            {
                return Ok();
            }

            logger.LogInformation("Received {Count} SendGrid events", events.Count);

            // Group events by newsletter ID
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
                        "Updated newsletter {NewsletterId} stats: delivered={Delivered}, opened={Opened}, clicked={Clicked}, bounced={Bounced}, unsubscribed={Unsubscribed}",
                        newsletterId, deliveredCount, openCount, clickCount, bounceCount, unsubscribeCount);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to update statistics for newsletter {NewsletterId}", newsletterId);
                }
            }

            return Ok();
        }
    }

    /// <summary>
    /// Model for SendGrid webhook events.
    /// </summary>
    public class SendGridEvent
    {
        /// <summary>
        /// Gets or sets the event type (delivered, open, click, bounce, dropped, unsubscribe, etc.).
        /// </summary>
        public string Event { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the SendGrid message ID.
        /// </summary>
        public string SgMessageId { get; set; }

        /// <summary>
        /// Gets or sets the newsletter ID from custom arguments.
        /// </summary>
        public string NewsletterId { get; set; }

        /// <summary>
        /// Gets the newsletter ID as a GUID if valid.
        /// </summary>
        public Guid? NewsletterIdValue
        {
            get
            {
                if (Guid.TryParse(NewsletterId, out var id))
                {
                    return id;
                }
                return null;
            }
        }

        /// <summary>
        /// Gets or sets the bounce reason if applicable.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets the clicked URL if applicable.
        /// </summary>
        public string Url { get; set; }
    }
}
