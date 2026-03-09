#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for rejecting attendee metrics.
/// </summary>
public class RejectMetricsRequestDto
{
    /// <summary>Gets or sets the rejection reason.</summary>
    public string RejectionReason { get; set; } = string.Empty;
}
