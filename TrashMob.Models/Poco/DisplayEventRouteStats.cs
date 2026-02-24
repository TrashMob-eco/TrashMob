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
        /// Gets or sets the total weight collected.
        /// </summary>
        public decimal TotalWeightCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight unit used for the aggregated total weight (defaults to pounds).
        /// </summary>
        public int TotalWeightUnitId { get; set; } = (int)TrashMob.Models.WeightUnitEnum.Pound;

        /// <summary>
        /// Gets or sets the estimated area covered in square meters, based on 25m grid cell aggregation.
        /// </summary>
        public double CoverageAreaSquareMeters { get; set; }

        /// <summary>
        /// Gets or sets the average litter density across all routes in grams per meter. Null if no routes have data.
        /// </summary>
        public double? AverageDensityGramsPerMeter { get; set; }

        /// <summary>
        /// Gets or sets the maximum litter density among all routes in grams per meter. Null if no routes have data.
        /// </summary>
        public double? MaxDensityGramsPerMeter { get; set; }
    }
}
