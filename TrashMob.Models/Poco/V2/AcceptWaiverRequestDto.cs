#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for accepting a waiver.
/// </summary>
public class AcceptWaiverRequestDto
{
    /// <summary>Gets or sets the waiver version to accept.</summary>
    public Guid WaiverVersionId { get; set; }

    /// <summary>Gets or sets the typed legal name.</summary>
    public string TypedLegalName { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the user is a minor.</summary>
    public bool IsMinor { get; set; }

    /// <summary>Gets or sets the guardian user ID (if minor and registered guardian).</summary>
    public Guid? GuardianUserId { get; set; }

    /// <summary>Gets or sets the guardian name (if minor).</summary>
    public string GuardianName { get; set; } = string.Empty;

    /// <summary>Gets or sets the guardian relationship (if minor).</summary>
    public string GuardianRelationship { get; set; } = string.Empty;
}
