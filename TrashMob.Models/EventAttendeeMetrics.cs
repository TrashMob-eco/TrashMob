#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents metrics submitted by an individual event attendee.
    /// </summary>
    /// <remarks>
    /// Allows attendees to submit their own bags collected, weight, and duration.
    /// Event leads can approve, reject, or adjust these submissions before they
    /// are included in event totals.
    /// </remarks>
    public class EventAttendeeMetrics : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event these metrics belong to.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who submitted these metrics.
        /// </summary>
        public Guid UserId { get; set; }

        /// <summary>
        /// Gets or sets the number of bags collected by this attendee.
        /// </summary>
        public int? BagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the weight of litter picked up by this attendee.
        /// </summary>
        public decimal? PickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the weight unit identifier for the picked weight.
        /// </summary>
        public int? PickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes this attendee participated.
        /// </summary>
        public int? DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets any notes from the attendee about their submission.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the approval status of these metrics.
        /// </summary>
        /// <remarks>
        /// Valid values: Pending, Approved, Rejected, Adjusted.
        /// </remarks>
        public string Status { get; set; } = "Pending";

        /// <summary>
        /// Gets or sets the identifier of the user who reviewed these metrics.
        /// </summary>
        public Guid? ReviewedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when these metrics were reviewed.
        /// </summary>
        public DateTimeOffset? ReviewedDate { get; set; }

        /// <summary>
        /// Gets or sets the reason for rejection, if applicable.
        /// </summary>
        public string RejectionReason { get; set; }

        /// <summary>
        /// Gets or sets the adjusted bags collected value set by the event lead.
        /// </summary>
        public int? AdjustedBagsCollected { get; set; }

        /// <summary>
        /// Gets or sets the adjusted weight value set by the event lead.
        /// </summary>
        public decimal? AdjustedPickedWeight { get; set; }

        /// <summary>
        /// Gets or sets the adjusted weight unit identifier set by the event lead.
        /// </summary>
        public int? AdjustedPickedWeightUnitId { get; set; }

        /// <summary>
        /// Gets or sets the adjusted duration in minutes set by the event lead.
        /// </summary>
        public int? AdjustedDurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the reason for adjustment by the event lead.
        /// </summary>
        public string AdjustmentReason { get; set; }

        /// <summary>
        /// Gets or sets the event these metrics belong to.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the user who submitted these metrics.
        /// </summary>
        public virtual User User { get; set; }

        /// <summary>
        /// Gets or sets the user who reviewed these metrics.
        /// </summary>
        public virtual User ReviewedByUser { get; set; }

        /// <summary>
        /// Gets or sets the weight unit for the picked weight.
        /// </summary>
        public virtual WeightUnit PickedWeightUnit { get; set; }

        /// <summary>
        /// Gets or sets the weight unit for the adjusted weight.
        /// </summary>
        public virtual WeightUnit AdjustedPickedWeightUnit { get; set; }
    }
}
