namespace TrashMob.Models.Poco
{
    using System;

    /// <summary>
    /// Represents an attendee's public metrics contribution for an event.
    /// Only shows approved/adjusted metrics where the attendee has opted in to public visibility.
    /// </summary>
    public class PublicAttendeeMetrics
    {
        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the user's display name.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the bags collected (adjusted value if available, otherwise original).
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight collected in pounds (adjusted value if available, otherwise original).
        /// </summary>
        public decimal? WeightPounds { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes (adjusted value if available, otherwise original).
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the approval status.
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// Represents the public metrics summary for an event including attendee breakdown.
    /// </summary>
    public class EventMetricsPublicSummary
    {
        /// <summary>
        /// Gets or sets the event ID.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the total bags collected from all approved submissions.
        /// </summary>
        public int TotalBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the total weight in pounds from all approved submissions.
        /// </summary>
        public decimal TotalWeightPounds { get; set; }

        /// <summary>
        /// Gets or sets the total duration in minutes from all approved submissions.
        /// </summary>
        public int TotalDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the count of attendees who submitted metrics.
        /// </summary>
        public int ContributorCount { get; set; }

        /// <summary>
        /// Gets or sets the list of public contributor metrics (those who opted in to public visibility).
        /// </summary>
        public System.Collections.Generic.List<PublicAttendeeMetrics> Contributors { get; set; } = new();
    }
}
