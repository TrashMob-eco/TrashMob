#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 DTO for newsletter data.
    /// </summary>
    public class NewsletterDto
    {
        /// <summary>Gets or sets the newsletter ID.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the category ID.</summary>
        public int CategoryId { get; set; }

        /// <summary>Gets or sets the category name.</summary>
        public string CategoryName { get; set; } = string.Empty;

        /// <summary>Gets or sets the email subject line.</summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>Gets or sets the preview text.</summary>
        public string PreviewText { get; set; } = string.Empty;

        /// <summary>Gets or sets the HTML content.</summary>
        public string HtmlContent { get; set; } = string.Empty;

        /// <summary>Gets or sets the plain text content.</summary>
        public string TextContent { get; set; } = string.Empty;

        /// <summary>Gets or sets the target type (All, Community, Team).</summary>
        public string TargetType { get; set; } = string.Empty;

        /// <summary>Gets or sets the target ID.</summary>
        public Guid? TargetId { get; set; }

        /// <summary>Gets or sets the newsletter status.</summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>Gets or sets the scheduled send date.</summary>
        public DateTimeOffset? ScheduledDate { get; set; }

        /// <summary>Gets or sets when the newsletter was sent.</summary>
        public DateTimeOffset? SentDate { get; set; }

        /// <summary>Gets or sets the total recipient count.</summary>
        public int RecipientCount { get; set; }

        /// <summary>Gets or sets the sent count.</summary>
        public int SentCount { get; set; }

        /// <summary>Gets or sets the delivered count.</summary>
        public int DeliveredCount { get; set; }

        /// <summary>Gets or sets the open count.</summary>
        public int OpenCount { get; set; }

        /// <summary>Gets or sets the click count.</summary>
        public int ClickCount { get; set; }

        /// <summary>Gets or sets the bounce count.</summary>
        public int BounceCount { get; set; }

        /// <summary>Gets or sets the unsubscribe count.</summary>
        public int UnsubscribeCount { get; set; }

        /// <summary>Gets or sets the created date.</summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>Gets or sets the last updated date.</summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
