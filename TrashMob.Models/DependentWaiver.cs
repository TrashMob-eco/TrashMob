#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a waiver signed by an adult on behalf of a dependent minor.
    /// </summary>
    /// <remarks>
    /// The adult must be the dependent's parent/guardian. The waiver text is
    /// snapshotted at signing time and expires at the end of the calendar year.
    /// </remarks>
    public class DependentWaiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the dependent this waiver covers.
        /// </summary>
        public Guid DependentId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the waiver version being signed.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the adult user who signed this waiver.
        /// </summary>
        public Guid SignedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name of the adult signer.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets the exact waiver text at the time of signing.
        /// </summary>
        public string WaiverTextSnapshot { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the waiver was signed.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets the date and time when this waiver expires.
        /// </summary>
        public DateTimeOffset ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the URL to the generated PDF document in immutable blob storage.
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Gets or sets the IP address at the time of signing for audit purposes.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent string at the time of signing for audit purposes.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets the dependent this waiver covers.
        /// </summary>
        public virtual Dependent Dependent { get; set; }

        /// <summary>
        /// Gets or sets the waiver version being signed.
        /// </summary>
        public virtual WaiverVersion WaiverVersion { get; set; }

        /// <summary>
        /// Gets or sets the adult user who signed this waiver.
        /// </summary>
        public virtual User SignedByUser { get; set; }

        /// <summary>
        /// Gets or sets the event registrations covered by this waiver.
        /// </summary>
        public virtual ICollection<EventDependent> EventDependents { get; set; }
    }
}
