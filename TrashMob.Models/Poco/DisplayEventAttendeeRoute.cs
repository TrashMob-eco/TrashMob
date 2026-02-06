namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents an event attendee route for display purposes with location points instead of geometric path.
    /// </summary>
    public class DisplayEventAttendeeRoute
    {
        /// <summary>
        /// Gets or sets the unique identifier for this route.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who took this route.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the route tracking.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the route tracking.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }

        /// <summary>
        /// Gets or sets the total distance in meters.
        /// </summary>
        public int TotalDistanceMeters { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the privacy level (Private, EventOnly, Public).
        /// </summary>
        public string PrivacyLevel { get; set; } = "EventOnly";

        /// <summary>
        /// Gets or sets whether the route has been trimmed for privacy.
        /// </summary>
        public bool IsTrimmed { get; set; }

        /// <summary>
        /// Gets or sets meters trimmed from the start.
        /// </summary>
        public int TrimStartMeters { get; set; }

        /// <summary>
        /// Gets or sets meters trimmed from the end.
        /// </summary>
        public int TrimEndMeters { get; set; }

        /// <summary>
        /// Gets or sets bags collected along this route.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets weight collected along this route in pounds.
        /// </summary>
        public decimal? WeightCollected { get; set; }

        /// <summary>
        /// Gets or sets notes about this route.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets when this route expires for public viewing.
        /// </summary>
        public DateTimeOffset? ExpiresDate { get; set; }

        /// <summary>
        /// Gets or sets the list of location points that make up the route, sorted by order.
        /// </summary>
        public List<SortableLocation> Locations { get; set; } = [];
    }
}