namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized community data for MCP responses.
/// Communities are partners with enabled home pages.
/// </summary>
public class CommunityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Tagline { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public string? Website { get; set; }
    public string Url { get; set; } = string.Empty;
}
