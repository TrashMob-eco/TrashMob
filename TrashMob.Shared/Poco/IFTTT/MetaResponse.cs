namespace TrashMob.Shared.Poco.IFTTT
{
    /// <summary>
    /// Represents metadata for an IFTTT response for IFTTT integration.
    /// </summary>
    public class MetaResponse
    {
        /// <summary>
        /// Gets or sets the unique identifier for the response item.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// Gets or sets the Unix timestamp of the response.
        /// </summary>
        public long timestamp { get; set; }
    }
}
