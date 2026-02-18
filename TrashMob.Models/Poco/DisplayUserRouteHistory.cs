namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a user's route with event context for route history display.
    /// </summary>
    public class DisplayUserRouteHistory
    {
        /// <summary>
        /// Gets or sets the route identifier.
        /// </summary>
        public Guid RouteId { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event date.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the total distance in meters.
        /// </summary>
        public int TotalDistanceMeters { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the privacy level.
        /// </summary>
        public string PrivacyLevel { get; set; } = "EventOnly";

        /// <summary>
        /// Gets or sets bags collected along this route.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets weight collected in pounds.
        /// </summary>
        public decimal? WeightCollected { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the event location.
        /// </summary>
        public double EventLatitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the event location.
        /// </summary>
        public double EventLongitude { get; set; }

        /// <summary>
        /// Gets or sets the start time of the route.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the route.
        /// </summary>
        public DateTimeOffset EndTime { get; set; }
    }
}
