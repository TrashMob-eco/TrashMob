#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// V2 API representation of an email template.
    /// </summary>
    public class EmailTemplateDto
    {
        /// <summary>Gets or sets the template name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Gets or sets the template content.</summary>
        public string Content { get; set; } = string.Empty;
    }
}
