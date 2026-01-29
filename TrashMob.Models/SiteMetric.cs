#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a site-wide metric measurement for tracking platform statistics.
    /// </summary>
    public class SiteMetric : KeyedModel
    {
        /// <summary>
        /// Gets or sets the date and time when the metric was processed.
        /// </summary>
        public DateTimeOffset ProcessedTime { get; set; }

        /// <summary>
        /// Gets or sets the type of metric being tracked.
        /// </summary>
        public string MetricType { get; set; }

        /// <summary>
        /// Gets or sets the value of the metric.
        /// </summary>
        public long MetricValue { get; set; }
    }
}