#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a contact request submitted by a user or visitor.
    /// </summary>
    public class ContactRequest : KeyedModel
    {
        /// <summary>
        /// Gets or sets the name of the person making the contact request.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the person making the contact request.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the message content of the contact request.
        /// </summary>
        public string Message { get; set; }
    }
}