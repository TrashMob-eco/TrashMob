#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents metrics submitted by an event attendee.
/// </summary>
public class EventAttendeeMetricsDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the event identifier.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets the user identifier.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets bags collected.</summary>
    public int? BagsCollected { get; set; }

    /// <summary>Gets or sets picked weight.</summary>
    public decimal? PickedWeight { get; set; }

    /// <summary>Gets or sets the weight unit identifier.</summary>
    public int? PickedWeightUnitId { get; set; }

    /// <summary>Gets or sets duration in minutes.</summary>
    public int? DurationMinutes { get; set; }

    /// <summary>Gets or sets notes.</summary>
    public string Notes { get; set; } = string.Empty;

    /// <summary>Gets or sets the status (Pending, Approved, Rejected, Adjusted).</summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>Gets or sets the reviewed date.</summary>
    public DateTimeOffset? ReviewedDate { get; set; }

    /// <summary>Gets or sets the rejection reason.</summary>
    public string RejectionReason { get; set; } = string.Empty;

    /// <summary>Gets or sets adjusted bags collected.</summary>
    public int? AdjustedBagsCollected { get; set; }

    /// <summary>Gets or sets adjusted picked weight.</summary>
    public decimal? AdjustedPickedWeight { get; set; }

    /// <summary>Gets or sets adjusted weight unit identifier.</summary>
    public int? AdjustedPickedWeightUnitId { get; set; }

    /// <summary>Gets or sets adjusted duration in minutes.</summary>
    public int? AdjustedDurationMinutes { get; set; }

    /// <summary>Gets or sets the adjustment reason.</summary>
    public string AdjustmentReason { get; set; } = string.Empty;
}
