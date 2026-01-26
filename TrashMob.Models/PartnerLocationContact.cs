#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a contact person at a partner location.
    /// </summary>
    public class PartnerLocationContact : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner location.
        /// </summary>
        public Guid PartnerLocationId { get; set; }

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
        /// Gets or sets the partner location this contact belongs to.
        /// </summary>
        public virtual PartnerLocation PartnerLocation { get; set; }
    }
}