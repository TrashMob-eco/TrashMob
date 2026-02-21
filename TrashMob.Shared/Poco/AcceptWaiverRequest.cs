namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Request model for accepting a waiver.
    /// </summary>
    public class AcceptWaiverRequest
    {
        /// <summary>
        /// Gets or sets the waiver version being accepted.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name entered by the signer.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the person signing the waiver.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the IP address from which the waiver was signed.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent from which the waiver was signed.
        /// </summary>
        public string UserAgent { get; set; }

        /// <summary>
        /// Gets or sets whether the signer is a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's user ID if the signer is a minor.
        /// </summary>
        public Guid? GuardianUserId { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name if not a registered user.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }
    }
}
