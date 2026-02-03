#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a unit of weight measurement (e.g., Pound, Kilogram).
    /// </summary>
    public class WeightUnit : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WeightUnit"/> class.
        /// </summary>
        public WeightUnit()
        {
            EventSummaries = new HashSet<EventSummary>();
            AttendeeMetricsForPicked = new HashSet<EventAttendeeMetrics>();
            AttendeeMetricsForAdjusted = new HashSet<EventAttendeeMetrics>();
        }

        /// <summary>
        /// Gets or sets the collection of event summaries using this weight unit.
        /// </summary>
        public virtual ICollection<EventSummary> EventSummaries { get; set; }

        /// <summary>
        /// Gets or sets the collection of attendee metrics using this weight unit for picked weight.
        /// </summary>
        public virtual ICollection<EventAttendeeMetrics> AttendeeMetricsForPicked { get; set; }

        /// <summary>
        /// Gets or sets the collection of attendee metrics using this weight unit for adjusted weight.
        /// </summary>
        public virtual ICollection<EventAttendeeMetrics> AttendeeMetricsForAdjusted { get; set; }
    }
}