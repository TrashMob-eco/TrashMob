namespace TrashMob.Shared.Poco.IFTTT
{
    /// <summary>
    /// Represents an event request containing location filters for IFTTT integration.
    /// </summary>
    public class IftttEventRequest
    {
        /// <summary>
        /// Gets or sets the city to filter events by.
        /// </summary>
        public string city { get; set; }

        /// <summary>
        /// Gets or sets the region or state to filter events by.
        /// </summary>
        public string region { get; set; }

        /// <summary>
        /// Gets or sets the postal code to filter events by.
        /// </summary>
        public string postal_code { get; set; }

        /// <summary>
        /// Gets or sets the country to filter events by.
        /// </summary>
        public string country { get; set; }
    }
}
