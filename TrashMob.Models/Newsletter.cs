#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a newsletter to be sent to subscribers.
    /// Supports sitewide, community, and team newsletters with scheduling and tracking.
    /// </summary>
    public class Newsletter : KeyedModel
    {
        /// <summary>
        /// Gets or sets the newsletter category identifier.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the preview text shown in email inbox.
        /// </summary>
        public string PreviewText { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of the newsletter.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content of the newsletter.
        /// </summary>
        public string TextContent { get; set; }

        #region Targeting

        /// <summary>
        /// Gets or sets the target type (All, Community, Team).
        /// </summary>
        public string TargetType { get; set; } = "All";

        /// <summary>
        /// Gets or sets the target identifier (community or team ID) when not targeting all users.
        /// </summary>
        public Guid? TargetId { get; set; }

        #endregion

        #region Status

        /// <summary>
        /// Gets or sets the newsletter status (Draft, Scheduled, Sending, Sent, Cancelled).
        /// </summary>
        public string Status { get; set; } = "Draft";

        /// <summary>
        /// Gets or sets the scheduled send date and time.
        /// </summary>
        public DateTimeOffset? ScheduledDate { get; set; }

        /// <summary>
        /// Gets or sets when the newsletter was actually sent.
        /// </summary>
        public DateTimeOffset? SentDate { get; set; }

        #endregion

        #region Statistics

        /// <summary>
        /// Gets or sets the total number of recipients.
        /// </summary>
        public int RecipientCount { get; set; }

        /// <summary>
        /// Gets or sets the count of successfully sent emails.
        /// </summary>
        public int SentCount { get; set; }

        /// <summary>
        /// Gets or sets the count of delivered emails.
        /// </summary>
        public int DeliveredCount { get; set; }

        /// <summary>
        /// Gets or sets the count of opened emails.
        /// </summary>
        public int OpenCount { get; set; }

        /// <summary>
        /// Gets or sets the count of emails with clicked links.
        /// </summary>
        public int ClickCount { get; set; }

        /// <summary>
        /// Gets or sets the count of bounced emails.
        /// </summary>
        public int BounceCount { get; set; }

        /// <summary>
        /// Gets or sets the count of unsubscribes from this newsletter.
        /// </summary>
        public int UnsubscribeCount { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the newsletter category navigation property.
        /// </summary>
        public virtual NewsletterCategory Category { get; set; }
    }
}
