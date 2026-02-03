namespace TrashMob.Shared.Poco
{
    /// <summary>
    /// Represents aggregated metrics totals from approved attendee submissions.
    /// </summary>
    public class AttendeeMetricsTotals
    {
        /// <summary>
        /// Gets or sets the total bags collected from approved submissions.
        /// </summary>
        public int TotalBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the total weight in pounds from approved submissions.
        /// </summary>
        /// <remarks>
        /// Weight is converted to pounds for consistent aggregation.
        /// Kilogram values are converted using 2.20462 lbs/kg.
        /// </remarks>
        public decimal TotalWeightPounds { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes from approved submissions.
        /// </summary>
        public int TotalDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the number of approved submissions.
        /// </summary>
        public int ApprovedSubmissions { get; set; }

        /// <summary>
        /// Gets or sets the number of pending submissions.
        /// </summary>
        public int PendingSubmissions { get; set; }

        /// <summary>
        /// Gets or sets the total number of submissions.
        /// </summary>
        public int TotalSubmissions { get; set; }
    }
}
