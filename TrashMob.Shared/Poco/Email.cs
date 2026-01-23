namespace TrashMob.Shared.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents an email address with an associated display name.
    /// </summary>
    public class EmailAddress
    {
        /// <summary>
        /// Gets or sets the display name associated with the email address.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address.
        /// </summary>
        public string Email { get; set; }
    }

    /// <summary>
    /// Represents an email message to be sent.
    /// </summary>
    public class Email
    {
        /// <summary>
        /// Gets the list of recipient email addresses.
        /// </summary>
        public List<EmailAddress> Addresses { get; } = new();

        /// <summary>
        /// Gets or sets the subject line of the email.
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// Gets or sets the plain text content of the email.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the HTML content of the email.
        /// </summary>
        public string HtmlMessage { get; set; }

        /// <summary>
        /// Gets or sets the dynamic template data for email template substitution.
        /// </summary>
        public object DynamicTemplateData { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the email template to use.
        /// </summary>
        public string TemplateId { get; set; }

        /// <summary>
        /// Gets or sets the group identifier for email categorization or unsubscribe handling.
        /// </summary>
        public int GroupId { get; set; }
    }
}