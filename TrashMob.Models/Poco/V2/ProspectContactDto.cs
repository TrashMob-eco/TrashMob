#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a prospect contact. Flat DTO excluding navigation properties.
    /// </summary>
    public class ProspectContactDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the prospect identifier.
        /// </summary>
        public Guid ProspectId { get; set; }

        /// <summary>
        /// Gets or sets the contact's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the contact's job title.
        /// </summary>
        public string? Title { get; set; }

        /// <summary>
        /// Gets or sets the contact email.
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Gets or sets the contact phone number.
        /// </summary>
        public string? Phone { get; set; }

        /// <summary>
        /// Gets or sets a free-text role descriptor.
        /// </summary>
        public string? Role { get; set; }

        /// <summary>
        /// Gets or sets the lifecycle status (Active, WrongPerson, NoResponse, LeftOrganization, RightPerson).
        /// </summary>
        public int ContactStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is the primary contact for the prospect.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the id of the contact that referred us to this one, if any.
        /// </summary>
        public Guid? ReferredByContactId { get; set; }

        /// <summary>
        /// Gets or sets notes about this contact.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets when the contact record was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the contact record was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
