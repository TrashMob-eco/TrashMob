namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized leaderboard entry for MCP responses.
/// Contains only public ranking information - no internal IDs.
/// </summary>
public class LeaderboardEntryDto
{
    public int Rank { get; set; }
    public string Name { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public decimal Score { get; set; }
    public string FormattedScore { get; set; } = string.Empty;
    public string? Region { get; set; }
    public string? City { get; set; }
}
