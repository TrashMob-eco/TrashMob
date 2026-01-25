namespace TrashMob.Shared.Poco.IFTTT
{
    using System;

    /// <summary>
    /// Represents an event response containing event details for IFTTT integration.
    /// </summary>
    public class IftttEventResponse : TriggersResponse
    {
        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string event_name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        public string event_id { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public DateTimeOffset event_date { get; set; }

        /// <summary>
        /// Gets or sets the street address where the event takes place.
        /// </summary>
        public string street_address { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city where the event takes place.
        /// </summary>
        public string city { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region or state where the event takes place.
        /// </summary>
        public string region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the event takes place.
        /// </summary>
        public string country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code where the event takes place.
        /// </summary>
        public string postal_code { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL to view event details.
        /// </summary>
        public string event_details_url { get; set; } = string.Empty;
    }
}
