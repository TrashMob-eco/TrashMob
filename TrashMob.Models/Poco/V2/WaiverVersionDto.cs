#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a waiver version for display.
/// </summary>
public class WaiverVersionDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the waiver name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the version string.</summary>
    public string Version { get; set; } = string.Empty;

    /// <summary>Gets or sets the waiver text (HTML).</summary>
    public string WaiverText { get; set; } = string.Empty;

    /// <summary>Gets or sets the effective date.</summary>
    public DateTimeOffset EffectiveDate { get; set; }

    /// <summary>Gets or sets the expiry date (null = current version).</summary>
    public DateTimeOffset? ExpiryDate { get; set; }

    /// <summary>Gets or sets whether active.</summary>
    public bool IsActive { get; set; }

    /// <summary>Gets or sets the scope (Global=0, Community=1).</summary>
    public WaiverScope Scope { get; set; }
}
