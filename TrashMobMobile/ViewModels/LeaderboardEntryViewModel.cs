namespace TrashMobMobile.ViewModels;

public class LeaderboardEntryViewModel
{
    public int Rank { get; set; }

    public string EntityName { get; set; } = string.Empty;

    public string FormattedScore { get; set; } = string.Empty;

    public bool IsCurrentUser { get; set; }

    public string RankDisplay => $"#{Rank}";
}
