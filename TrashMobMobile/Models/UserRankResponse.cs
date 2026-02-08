namespace TrashMobMobile.Models;

public class UserRankResponse
{
    public string LeaderboardType { get; set; } = string.Empty;

    public string TimeRange { get; set; } = string.Empty;

    public int? Rank { get; set; }

    public decimal? Score { get; set; }

    public string FormattedScore { get; set; } = string.Empty;

    public int TotalRanked { get; set; }

    public bool IsEligible { get; set; }

    public string IneligibleReason { get; set; } = string.Empty;
}
