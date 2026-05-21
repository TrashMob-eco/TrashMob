#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// A person (or generic mailbox) at a CommunityProspect organization. A prospect
    /// can have multiple contacts so outreach history is preserved as we try different
    /// people before finding the right one.
    /// </summary>
    public class ProspectContact : KeyedModel
    {
        /// <summary>
        /// Gets or sets the prospect this contact belongs to.
        /// </summary>
        public Guid ProspectId { get; set; }

        /// <summary>
        /// Gets or sets the contact's name (person or mailbox label).
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the contact's job title at the prospect organization.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the contact email address.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the contact phone number.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets a free-text role descriptor (e.g., "Decision-maker", "Gatekeeper", "Referral").
        /// </summary>
        public string Role { get; set; }

        /// <summary>
        /// Gets or sets the current status of this contact. See <see cref="ProspectContactStatus"/>.
        /// </summary>
        public int ContactStatus { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this contact is the current best (primary)
        /// contact for the prospect. At most one ProspectContact per prospect should have IsPrimary=true.
        /// </summary>
        public bool IsPrimary { get; set; }

        /// <summary>
        /// Gets or sets the id of the ProspectContact that pointed us at this contact, if any.
        /// </summary>
        public Guid? ReferredByContactId { get; set; }

        /// <summary>
        /// Gets or sets free-form notes about this specific person.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the parent prospect navigation property.
        /// </summary>
        public virtual CommunityProspect Prospect { get; set; }

        /// <summary>
        /// Gets or sets the contact who referred us to this contact, if any.
        /// </summary>
        public virtual ProspectContact ReferredByContact { get; set; }
    }

    /// <summary>
    /// Lifecycle status for a <see cref="ProspectContact"/>.
    /// </summary>
    public enum ProspectContactStatus
    {
        /// <summary>Default state — this contact is in play.</summary>
        Active = 0,

        /// <summary>Contact responded but is not the right person to talk to.</summary>
        WrongPerson = 1,

        /// <summary>Contact has not responded to multiple outreach attempts.</summary>
        NoResponse = 2,

        /// <summary>Contact has left the organization (per their reply or a bounce).</summary>
        LeftOrganization = 3,

        /// <summary>Contact is confirmed as the right decision-maker.</summary>
        RightPerson = 4,
    }
}
