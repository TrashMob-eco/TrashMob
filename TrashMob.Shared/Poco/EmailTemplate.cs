namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents an email template with a name and content.
    /// </summary>
    public class EmailTemplate
    {
        /// <summary>
        /// Gets or sets the name of the email template.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the content of the email template.
        /// </summary>
        public string Content { get; set; }
    }
}