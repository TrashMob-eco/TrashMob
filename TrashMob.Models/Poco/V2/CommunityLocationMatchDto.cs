#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// A lightweight response for GET /api/v2/communities/at-location — enough to
/// display the "contributing to [Community Name]" banner and link to the
/// community page, without pulling the full Partner DTO. See Project 65 Phase 3.
/// </summary>
public class CommunityLocationMatchDto
{
    /// <summary>The community's partner id.</summary>
    public Guid Id { get; set; }

    /// <summary>Display name (e.g. "Seattle, WA").</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>URL slug for the community page (e.g. "seattle-wa").</summary>
    public string Slug { get; set; } = string.Empty;
}
