#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a document associated with a partner organization.
    /// </summary>
    public class PartnerDocument : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the document.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the URL where the document is stored (for external link documents).
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the Azure Blob Storage path for uploaded documents. Null for external URL documents.
        /// </summary>
        public string BlobStoragePath { get; set; }

        /// <summary>
        /// Gets or sets the MIME content type of the uploaded document (e.g., application/pdf).
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Gets or sets the file size in bytes for uploaded documents.
        /// </summary>
        public long? FileSizeBytes { get; set; }

        /// <summary>
        /// Gets or sets the document type identifier. Maps to <see cref="PartnerDocumentTypeEnum"/>.
        /// </summary>
        public int DocumentTypeId { get; set; }

        /// <summary>
        /// Gets or sets the optional expiration date for the document.
        /// </summary>
        public DateTimeOffset? ExpirationDate { get; set; }

        /// <summary>
        /// Gets or sets the partner organization this document belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }
    }
}