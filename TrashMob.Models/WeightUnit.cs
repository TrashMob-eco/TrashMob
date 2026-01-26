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
        }

        /// <summary>
        /// Gets or sets the collection of event summaries using this weight unit.
        /// </summary>
        public virtual ICollection<EventSummary> EventSummaries { get; set; }
    }
}