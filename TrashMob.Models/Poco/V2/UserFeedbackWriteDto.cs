namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Write DTO for submitting user feedback.
    /// </summary>
    public class UserFeedbackWriteDto
    {
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
    }
}
