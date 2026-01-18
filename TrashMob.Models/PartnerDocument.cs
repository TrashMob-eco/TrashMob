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
        /// Gets or sets the URL where the document is stored.
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// Gets or sets the partner organization this document belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }
    }
}