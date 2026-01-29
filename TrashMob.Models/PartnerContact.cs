#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a contact person for a partner organization.
    /// </summary>
    public class PartnerContact : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the contact person.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address of the contact person.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the phone number of the contact person.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets any notes about the contact person.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this contact is active.
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Gets or sets the partner organization this contact belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }
    }
}