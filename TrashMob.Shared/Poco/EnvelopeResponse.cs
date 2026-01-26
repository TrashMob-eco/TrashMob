namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents a response from creating a document signing envelope.
    /// </summary>
    public class EnvelopeResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier of the created envelope.
        /// </summary>
        public string EnvelopeId { get; set; }

        /// <summary>
        /// Gets or sets the URL to redirect the user to for document signing.
        /// </summary>
        public string RedirectUrl { get; set; }
    }
}