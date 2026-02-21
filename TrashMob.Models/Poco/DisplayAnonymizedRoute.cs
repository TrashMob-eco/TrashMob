namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a route without user identity for public event views.
    /// </summary>
    public class DisplayAnonymizedRoute
    {
        /// <summary>
        /// Gets or sets the route identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the start time of the route.
        /// </summary>
        public DateTimeOffset StartTime { get; set; }

        /// <summary>
        /// Gets or sets the end time of the route.
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
        /// Gets or sets bags collected along this route.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets weight collected in pounds.
        /// </summary>
        public decimal? WeightCollected { get; set; }

        /// <summary>
        /// Gets or sets the list of location points that make up the route.
        /// </summary>
        public List<SortableLocation> Locations { get; set; } = [];
    }
}
