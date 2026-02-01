#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents user feedback submitted through the website feedback widget.
    /// </summary>
    /// <remarks>
    /// Feedback can be submitted by both authenticated users and anonymous visitors.
    /// For anonymous submissions, UserId will be null and Email may be provided for follow-up.
    /// Categories: Bug, FeatureRequest, General, Praise
    /// Status transitions: New (default) → Reviewed → Resolved or Deferred
    /// </remarks>
    public class UserFeedback : KeyedModel
    {
        /// <summary>
        /// Gets or sets the optional user ID if the submitter is logged in.
        /// </summary>
        public Guid? UserId { get; set; }

        /// <summary>
        /// Gets or sets the feedback category.
        /// </summary>
        /// <remarks>
        /// Valid values: Bug, FeatureRequest, General, Praise
        /// </remarks>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the feedback description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the optional email address for anonymous users to receive follow-up.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob Storage URL for an optional screenshot.
        /// </summary>
        public string ScreenshotUrl { get; set; }

        /// <summary>
        /// Gets or sets the page URL where the feedback was submitted from.
        /// </summary>
        public string PageUrl { get; set; }

        /// <summary>
        /// Gets or sets the user agent string of the browser.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the feedback status.
        /// </summary>
        /// <remarks>
        /// Valid values: New, Reviewed, Resolved, Deferred
        /// </remarks>
        public string Status { get; set; } = "New";

        /// <summary>
        /// Gets or sets internal notes added by administrators.
        /// </summary>
        public string InternalNotes { get; set; }

        /// <summary>
        /// Gets or sets the ID of the admin user who reviewed this feedback.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date when this feedback was reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the GitHub issue URL if this feedback was converted to an issue.
        /// </summary>
        public string GitHubIssueUrl { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted this feedback (if logged in).
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the admin user who reviewed this feedback.
        /// </summary>
        public virtual User ReviewedByUser { get; set; }
    }
}
