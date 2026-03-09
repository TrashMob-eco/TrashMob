#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Result of checking whether a user has a valid waiver.
/// </summary>
public class WaiverCheckResultDto
{
    /// <summary>Gets or sets whether the user has a valid waiver.</summary>
    public bool HasValidWaiver { get; set; }
}
