#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a minor (dependent) linked to a registered adult's account.
    /// </summary>
    /// <remarks>
    /// Adults manage dependent profiles to bring children to cleanup events
    /// without the children needing their own accounts (COPPA avoidance).
    /// </remarks>
    public class Dependent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the parent/guardian user who manages this dependent.
        /// </summary>
        public Guid ParentUserId { get; set; }

        /// <summary>
        /// Gets or sets the dependent's first name.
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the dependent's last name.
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the dependent's date of birth for age verification and tier enforcement.
        /// </summary>
        public DateOnly DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets the adult's relationship to the dependent.
        /// </summary>
        /// <remarks>
        /// Valid values: parent, legal guardian, grandparent, authorized supervisor, other.
        /// </remarks>
        public string Relationship { get; set; }

        /// <summary>
        /// Gets or sets optional medical notes (allergies, conditions, medications).
        /// </summary>
        public string MedicalNotes { get; set; }

        /// <summary>
        /// Gets or sets an optional emergency contact phone if different from the parent's.
        /// </summary>
        public string EmergencyContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the dependent's email address (optional). Used for PRIVO consent and account invitation.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets whether this dependent profile is active (soft-delete support).
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the PRIVO Service Identifier (SiD) for this dependent.
        /// </summary>
        public string PrivoSid { get; set; }

        /// <summary>
        /// Gets or sets the PRIVO parental consent record ID for this dependent.
        /// </summary>
        public Guid? ParentalConsentId { get; set; }

        /// <summary>
        /// Gets or sets the parent/guardian user.
        /// </summary>
        public virtual User ParentUser { get; set; }

        /// <summary>
        /// Gets or sets the event registrations for this dependent.
        /// </summary>
        public virtual ICollection<EventDependent> EventDependents { get; set; }

        /// <summary>
        /// Gets or sets the waivers signed for this dependent.
        /// </summary>
        public virtual ICollection<DependentWaiver> DependentWaivers { get; set; }

        /// <summary>
        /// Gets or sets the invitations sent for this dependent to create an account.
        /// </summary>
        public virtual ICollection<DependentInvitation> DependentInvitations { get; set; }

        /// <summary>
        /// Gets or sets the PRIVO parental consent record for this dependent.
        /// </summary>
        public virtual ParentalConsent ParentalConsent { get; set; }
    }
}
