namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a user's personal impact statistics across all events.
    /// </summary>
    public class UserImpactStats
    {
        /// <summary>
        /// Gets or sets the total bags collected across all approved submissions.
        /// </summary>
        public int TotalBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the total weight in pounds across all approved submissions.
        /// </summary>
        public decimal TotalWeightPounds { get; set; }

        /// <summary>
        /// Gets or sets the total weight in kilograms across all approved submissions.
        /// </summary>
        public decimal TotalWeightKilograms { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes across all approved submissions.
        /// </summary>
        public int TotalDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the total number of events with approved metrics.
        /// </summary>
        public int EventsWithMetrics { get; set; }

        /// <summary>
        /// Gets or sets the per-event breakdown of approved metrics.
        /// </summary>
        public List<UserEventMetricsSummary> EventBreakdown { get; set; } = new();
    }

    /// <summary>
    /// Represents a summary of a user's metrics for a single event.
    /// </summary>
    public class UserEventMetricsSummary
    {
        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        public System.Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event date.
        /// </summary>
        public System.DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the bags collected at this event.
        /// </summary>
        public int BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight collected at this event (in pounds).
        /// </summary>
        public decimal WeightPounds { get; set; }

        /// <summary>
        /// Gets or sets the duration at this event (in minutes).
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the approval status.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}
