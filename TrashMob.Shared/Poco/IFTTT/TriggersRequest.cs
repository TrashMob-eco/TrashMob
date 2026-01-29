namespace TrashMob.Shared.Poco.IFTTT
{
    /// <summary>
    /// Represents a triggers request for firing IFTTT triggers for IFTTT integration.
    /// </summary>
    public class TriggersRequest
    {
        /// <summary>
        /// Gets or sets the identity of the trigger being invoked.
        /// </summary>
        public string trigger_identity { get; set; }

        /// <summary>
        /// Gets or sets the trigger fields containing filter criteria.
        /// </summary>
        public object triggerFields { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of results to return. Defaults to 50.
        /// </summary>
        public int limit { get; set; } = 50;

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
