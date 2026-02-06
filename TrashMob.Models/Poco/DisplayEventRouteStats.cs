namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Aggregated route statistics for an event.
    /// </summary>
    public class DisplayEventRouteStats
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the total number of routes recorded.
        /// </summary>
        public int TotalRoutes { get; set; }

        /// <summary>
        /// Gets or sets the total distance covered in meters.
        /// </summary>
        public long TotalDistanceMeters { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes.
        /// </summary>
        public long TotalDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the number of unique contributors.
        /// </summary>
        public int UniqueContributors { get; set; }

        /// <summary>
        /// Gets or sets the total bags collected.
        /// </summary>
        public int TotalBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the total weight collected in pounds.
        /// </summary>
        public decimal TotalWeightCollected { get; set; }
    }
}
