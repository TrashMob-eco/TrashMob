#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// V2 API representation of a waiver definition.
/// </summary>
public class WaiverDto
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the name of the waiver.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the waiver is enabled and required.</summary>
    public bool IsWaiverEnabled { get; set; }
}
