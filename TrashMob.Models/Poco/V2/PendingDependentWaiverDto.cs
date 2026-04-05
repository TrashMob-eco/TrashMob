#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a dependent's pending waiver request for a specific event.
/// </summary>
public class PendingDependentWaiverDto
{
    /// <summary>Gets or sets the dependent ID.</summary>
    public Guid DependentId { get; set; }

    /// <summary>Gets or sets the dependent's first name.</summary>
    public string DependentFirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the dependent's last name.</summary>
    public string DependentLastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the event ID.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets the event name.</summary>
    public string EventName { get; set; } = string.Empty;

    /// <summary>Gets or sets the event date.</summary>
    public DateTimeOffset EventDate { get; set; }

    /// <summary>Gets or sets the minor user ID (the child's account).</summary>
    public Guid MinorUserId { get; set; }

    /// <summary>Gets or sets the waiver versions that need signing.</summary>
    public List<WaiverVersionSummaryDto> RequiredWaivers { get; set; } = [];
}

/// <summary>
/// Summary of a waiver version that needs signing.
/// </summary>
public class WaiverVersionSummaryDto
{
    /// <summary>Gets or sets the waiver version ID.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the waiver name/title.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the waiver scope (Global or Community).</summary>
    public string Scope { get; set; } = string.Empty;
}
