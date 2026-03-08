#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// Query parameters for filtering and paginating events in the V2 API.
    /// </summary>
    public class EventQueryParameters : QueryParameters
    {
        /// <summary>
        /// Gets or sets the optional event status identifier to filter by.
        /// </summary>
        public int? EventStatusId { get; set; }

        /// <summary>
        /// Gets or sets the optional event type identifier to filter by.
        /// </summary>
        public int? EventTypeId { get; set; }

        /// <summary>
        /// Gets or sets the optional city to filter by.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the optional region (state/province) to filter by.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the optional country to filter by.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the start of the date range to filter events.
        /// </summary>
        public DateTimeOffset? FromDate { get; set; }

        /// <summary>
        /// Gets or sets the end of the date range to filter events.
        /// </summary>
        public DateTimeOffset? ToDate { get; set; }
    }
}
