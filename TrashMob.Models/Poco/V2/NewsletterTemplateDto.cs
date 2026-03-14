#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 DTO for newsletter template data.
    /// </summary>
    public class NewsletterTemplateDto
    {
        /// <summary>Gets or sets the template ID.</summary>
        public int Id { get; set; }

        /// <summary>Gets or sets the template name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the template description.</summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>Gets or sets the HTML content template.</summary>
        public string HtmlContent { get; set; } = string.Empty;

        /// <summary>Gets or sets the plain text content template.</summary>
        public string TextContent { get; set; } = string.Empty;

        /// <summary>Gets or sets a URL to a thumbnail preview.</summary>
        public string? ThumbnailUrl { get; set; }
    }
}
