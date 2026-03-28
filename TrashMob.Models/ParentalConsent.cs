#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Records a PRIVO consent request for adult identity verification or parental consent for a minor.
    /// </summary>
    public class ParentalConsent : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user this consent applies to (adult for verification, child for parental consent).
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the parent/guardian user ID (for child consent flows).
        /// </summary>
        public Guid? ParentUserId { get; set; }

        /// <summary>
        /// Gets or sets the dependent record ID (for parent-initiated child consent).
        /// </summary>
        public Guid? DependentId { get; set; }

        /// <summary>
        /// Gets or sets the PRIVO consent identifier returned from the /requests API.
        /// </summary>
        public string PrivoConsentIdentifier { get; set; }

        /// <summary>
        /// Gets or sets the PRIVO Service Identifier (SiD) for the principal user.
        /// </summary>
        public string PrivoSid { get; set; }

        /// <summary>
        /// Gets or sets the PRIVO Service Identifier (SiD) for the granter (parent/guardian).
        /// </summary>
        public string PrivoGranterSid { get; set; }

        /// <summary>
        /// Gets or sets the type of consent request.
        /// </summary>
        public ConsentType ConsentType { get; set; }

        /// <summary>
        /// Gets or sets the current status of the consent request.
        /// </summary>
        public ConsentStatus Status { get; set; } = ConsentStatus.Pending;

        /// <summary>
        /// Gets or sets the PRIVO consent URL for redirect (transient, used during flow initiation).
        /// </summary>
        public string ConsentUrl { get; set; }

        /// <summary>
        /// Gets or sets when consent was verified/approved.
        /// </summary>
        public DateTimeOffset? VerifiedDate { get; set; }

        /// <summary>
        /// Gets or sets when consent was revoked.
        /// </summary>
        public DateTimeOffset? RevokedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for consent revocation.
        /// </summary>
        public string RevokedReason { get; set; }

        // Navigation properties

        /// <summary>
        /// Gets or sets the user this consent applies to.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the parent/guardian user.
        /// </summary>
        public virtual User ParentUser { get; set; }

        /// <summary>
        /// Gets or sets the dependent record.
        /// </summary>
        public virtual Dependent Dependent { get; set; }
    }
}
