#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 DTO for updating a newsletter draft.
    /// </summary>
    public class UpdateNewsletterDto
    {
        /// <summary>Gets or sets the category ID.</summary>
        public int? CategoryId { get; set; }

        /// <summary>Gets or sets the email subject line.</summary>
        public string? Subject { get; set; }

        /// <summary>Gets or sets the preview text.</summary>
        public string? PreviewText { get; set; }

        /// <summary>Gets or sets the HTML content.</summary>
        public string? HtmlContent { get; set; }

        /// <summary>Gets or sets the plain text content.</summary>
        public string? TextContent { get; set; }

        /// <summary>Gets or sets the target type (All, Community, Team).</summary>
        public string? TargetType { get; set; }

        /// <summary>Gets or sets the target ID.</summary>
        public Guid? TargetId { get; set; }
    }
}
