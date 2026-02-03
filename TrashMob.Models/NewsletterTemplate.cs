#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a reusable newsletter template with pre-defined layouts.
    /// Templates provide consistent branding and reduce composition time.
    /// </summary>
    public class NewsletterTemplate : LookupModel
    {
        /// <summary>
        /// Gets or sets the HTML content template with placeholders.
        /// Supports tokens like {{content}}, {{unsubscribe_url}}, {{preferences_url}}.
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Gets or sets the plain text content template with placeholders.
        /// </summary>
        public string TextContent { get; set; }

        /// <summary>
        /// Gets or sets a URL to a thumbnail preview of the template.
        /// </summary>
        public string ThumbnailUrl { get; set; }
    }
}
