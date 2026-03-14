namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Read DTO for user feedback (admin view).
    /// </summary>
    public class UserFeedbackDto
    {
        /// <summary>Gets or sets the feedback ID.</summary>
        public Guid Id { get; set; }

        /// <summary>Gets or sets the submitter's user ID (null if anonymous).</summary>
        public Guid? UserId { get; set; }

        /// <summary>Gets or sets the feedback category (Bug, FeatureRequest, General, Praise).</summary>
        public string Category { get; set; } = string.Empty;

        /// <summary>Gets or sets the feedback description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the optional email for anonymous follow-up.</summary>
        public string? Email { get; set; }

        /// <summary>Gets or sets the screenshot URL.</summary>
        public string? ScreenshotUrl { get; set; }

        /// <summary>Gets or sets the page URL where feedback was submitted.</summary>
        public string? PageUrl { get; set; }

        /// <summary>Gets or sets the user agent string.</summary>
        public string? UserAgent { get; set; }

        /// <summary>Gets or sets the feedback status (New, Reviewed, Resolved, Deferred).</summary>
        public string Status { get; set; } = "New";

        /// <summary>Gets or sets internal admin notes.</summary>
        public string? InternalNotes { get; set; }

        /// <summary>Gets or sets the reviewer's user ID.</summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>Gets or sets the review date.</summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>Gets or sets the linked GitHub issue URL.</summary>
        public string? GitHubIssueUrl { get; set; }

        /// <summary>Gets or sets the creation date.</summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
