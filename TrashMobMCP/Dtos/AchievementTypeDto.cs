namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized achievement type for MCP responses.
/// Describes an available badge/achievement users can earn.
/// </summary>
public class AchievementTypeDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string? IconUrl { get; set; }
    public int Points { get; set; }
    public string? Criteria { get; set; }
}
