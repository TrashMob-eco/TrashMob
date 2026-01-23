namespace TrashMob.Shared.Poco.IFTTT
{
    /// <summary>
    /// Represents a queries request for retrieving trigger data for IFTTT integration.
    /// </summary>
    public class QueriesRequest
    {
        /// <summary>
        /// Gets or sets the identity of the trigger being queried.
        /// </summary>
        public string trigger_identity { get; set; }

        /// <summary>
        /// Gets or sets the trigger fields containing filter criteria.
        /// </summary>
        public object triggerFields { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to return.
        /// </summary>
        public int limit { get; set; }

        /// <summary>
        /// Gets or sets the user information associated with the request.
        /// </summary>
        public object user { get; set; }

        /// <summary>
        /// Gets or sets the IFTTT source information.
        /// </summary>
        public object ifttt_source { get; set; }
    }
}
