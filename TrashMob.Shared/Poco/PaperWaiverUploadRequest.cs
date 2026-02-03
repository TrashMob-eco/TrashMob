namespace TrashMob.Shared.Poco
{
    using System;
    using Microsoft.AspNetCore.Http;

    /// <summary>
    /// Request model for uploading a paper waiver on behalf of a user.
    /// </summary>
    public class PaperWaiverUploadRequest
    {
        /// <summary>
        /// Gets or sets the uploaded file (PDF, JPEG, PNG, or WebP).
        /// </summary>
        public IFormFile FormFile { get; set; }

        /// <summary>
        /// Gets or sets the user ID of the person who signed the waiver.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the waiver version being signed.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets the name as written on the paper waiver.
        /// </summary>
        public string SignerName { get; set; }

        /// <summary>
        /// Gets or sets the date the paper waiver was signed.
        /// </summary>
        public DateTimeOffset DateSigned { get; set; }

        /// <summary>
        /// Gets or sets the optional event ID if uploading for a specific event.
        /// </summary>
        public Guid? EventId { get; set; }

        /// <summary>
        /// Gets or sets whether the signer is a minor.
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the guardian's name if the signer is a minor.
        /// </summary>
        public string GuardianName { get; set; }

        /// <summary>
        /// Gets or sets the guardian's relationship to the minor.
        /// </summary>
        public string GuardianRelationship { get; set; }
    }
}
