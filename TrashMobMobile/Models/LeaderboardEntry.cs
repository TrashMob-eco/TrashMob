namespace TrashMobMobile.Models;

public class LeaderboardEntry
{
    public Guid EntityId { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string EntityType { get; set; } = string.Empty;

    public int Rank { get; set; }

    public decimal Score { get; set; }

    public string FormattedScore { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;
}
