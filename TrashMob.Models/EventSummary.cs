#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a summary of an event's outcomes after completion.
    /// </summary>
    public class EventSummary : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the number of buckets filled during the event.
        /// </summary>
        public int NumberOfBuckets { get; set; }

        /// <summary>
        /// Gets or sets the number of bags filled during the event.
        /// </summary>
        public int NumberOfBags { get; set; }

        /// <summary>
        /// Gets or sets the actual duration of the event in minutes.
        /// </summary>
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the actual number of attendees who participated in the event.
        /// </summary>
        public int ActualNumberOfAttendees { get; set; }

        /// <summary>
        /// Gets or sets the total weight of trash picked up during the event.
        /// </summary>
        public int PickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the weight unit used for the picked weight.
        /// </summary>
        public int PickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets any additional notes about the event.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the event associated with this summary.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the weight unit used for the picked weight measurement.
        /// </summary>
        public virtual WeightUnit PickedWeightUnit { get; set; }
    }
}