#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Records a user's acceptance of a waiver with comprehensive audit trail.
    /// </summary>
    public class UserWaiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the user who accepted the waiver.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the waiver version that was accepted.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets when the waiver was accepted/signed.
        /// </summary>
        public DateTimeOffset AcceptedDate { get; set; }

        /// <summary>
        /// Gets or sets when this acceptance expires (typically end of calendar year).
        /// </summary>
        public DateTimeOffset ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets the typed legal name entered by the signer.
        /// </summary>
        public string TypedLegalName { get; set; }

        /// <summary>
        /// Gets or sets the full waiver text at time of signing (snapshot for PDF generation).
        /// </summary>
        public string WaiverTextSnapshot { get; set; }

        #region Signing Method

        /// <summary>
        /// Gets or sets how the waiver was signed (ESignatureWeb, ESignatureMobile, PaperUpload).
        /// </summary>
        public string SigningMethod { get; set; }

        /// <summary>
        /// Gets or sets the URL to the generated PDF in immutable blob storage.
        /// </summary>
        public string DocumentUrl { get; set; }

        /// <summary>
        /// Gets or sets who uploaded the paper waiver (null for e-signatures).
        /// </summary>
        public Guid? UploadedByUserId { get; set; }

        #endregion

        #region Audit Trail

        /// <summary>
        /// Gets or sets the IP address at time of acceptance.
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// Gets or sets the user agent at time of acceptance.
        /// </summary>
        public string UserAgent { get; set; }

        #endregion

        #region Minor Support

        /// <summary>
        /// Gets or sets whether the user was a minor at time of acceptance.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's user ID (if minor and guardian is a registered user).
        /// </summary>
        public Guid? GuardianUserId { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name (if minor and guardian not a registered user).
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }

        #endregion

        #region Navigation Properties

        /// <summary>
        /// Gets or sets the user navigation property.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the waiver version navigation property.
        /// </summary>
        public virtual WaiverVersion WaiverVersion { get; set; }

        /// <summary>
        /// Gets or sets the user who uploaded the paper waiver (navigation property).
        /// </summary>
        public virtual User UploadedByUser { get; set; }

        /// <summary>
        /// Gets or sets the guardian user navigation property.
        /// </summary>
        public virtual User GuardianUser { get; set; }

        #endregion
    }
}
