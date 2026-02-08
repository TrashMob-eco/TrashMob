namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public class LeaderboardManager(ILeaderboardRestService leaderboardRestService) : ILeaderboardManager
{
    public Task<LeaderboardResponse> GetLeaderboardAsync(string type = "Events",
        string timeRange = "Month", int limit = 50, CancellationToken cancellationToken = default)
    {
        return leaderboardRestService.GetLeaderboardAsync(type, timeRange, limit, cancellationToken);
    }

    public Task<UserRankResponse> GetMyRankAsync(string type = "Events",
        string timeRange = "AllTime", CancellationToken cancellationToken = default)
    {
        return leaderboardRestService.GetMyRankAsync(type, timeRange, cancellationToken);
    }

    public Task<LeaderboardResponse> GetTeamLeaderboardAsync(string type = "Events",
        string timeRange = "Month", int limit = 50, CancellationToken cancellationToken = default)
    {
        return leaderboardRestService.GetTeamLeaderboardAsync(type, timeRange, limit, cancellationToken);
    }
}
