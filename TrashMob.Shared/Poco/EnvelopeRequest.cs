namespace TrashMob.Shared.Poco
{
    using TrashMob.Models;

    /// <summary>
    /// Represents a request to create a document signing envelope.
    /// </summary>
    public class EnvelopeRequest : BaseModel
    {
        /// <summary>
        /// Gets or sets the email address of the document signer.
        /// </summary>
        public string SignerEmail { get; set; }

        /// <summary>
        /// Gets or sets the name of the document signer.
        /// </summary>
        public string SignerName { get; set; }

        /// <summary>
        /// Gets or sets the URL to redirect the user to after signing is complete.
        /// </summary>
        public string ReturnUrl { get; set; }

        /// <summary>
        /// Gets or sets the URL to ping for webhook notifications about signing status.
        /// </summary>
        public string PingUrl { get; set; }
    }
}