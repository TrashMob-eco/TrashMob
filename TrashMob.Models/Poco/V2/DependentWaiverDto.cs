#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a dependent waiver. Excludes internal tracking fields (IP, UserAgent).
    /// </summary>
    public class DependentWaiverDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the waiver.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the dependent identifier.
        /// </summary>
        public Guid DependentId { get; set; }

        /// <summary>
        /// Gets or sets the waiver version identifier.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the user identifier of who signed the waiver.
        /// </summary>
        public Guid SignedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name used when signing.
        /// </summary>
        public string TypedLegalName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date the waiver was accepted.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets the date the waiver expires.
        /// </summary>
        public DateTimeOffset ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the URL of the signed waiver document.
        /// </summary>
        public string DocumentUrl { get; set; } = string.Empty;
    }
}
