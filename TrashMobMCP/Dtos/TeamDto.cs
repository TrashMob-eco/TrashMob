namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized team data for MCP responses.
/// Contains only public information.
/// </summary>
public class TeamDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? City { get; set; }
    public string? Region { get; set; }
    public string? Country { get; set; }
    public int MemberCount { get; set; }
    public bool IsActive { get; set; }
    public string Url { get; set; } = string.Empty;
}
