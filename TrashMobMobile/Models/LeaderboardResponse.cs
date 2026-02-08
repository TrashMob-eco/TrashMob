namespace TrashMobMobile.Models;

public class LeaderboardResponse
{
    public string LeaderboardType { get; set; } = string.Empty;

    public string TimeRange { get; set; } = string.Empty;

    public string LocationScope { get; set; } = string.Empty;

    public string LocationValue { get; set; } = string.Empty;

    public DateTimeOffset ComputedDate { get; set; }

    public int TotalEntries { get; set; }

    public List<LeaderboardEntry> Entries { get; set; } = [];
}
