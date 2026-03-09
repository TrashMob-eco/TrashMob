#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an event's post-completion summary metrics.
    /// </summary>
    public class EventSummaryDto
    {
        /// <summary>
        /// Gets or sets the event identifier.
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
        /// Gets or sets the actual number of attendees who participated.
        /// </summary>
        public int ActualNumberOfAttendees { get; set; }

        /// <summary>
        /// Gets or sets the total weight of trash collected.
        /// </summary>
        public decimal PickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the weight unit identifier (Pound or Kilogram).
        /// </summary>
        public int PickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets whether this summary was auto-populated from route data.
        /// </summary>
        public bool IsFromRouteData { get; set; }

        /// <summary>
        /// Gets or sets any additional notes about the event.
        /// </summary>
        public string Notes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets when the summary was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the summary was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
