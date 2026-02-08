namespace TrashMobMobile.Services;

using TrashMobMobile.Models;

public interface ILeaderboardRestService
{
    Task<LeaderboardResponse> GetLeaderboardAsync(string type = "Events", string timeRange = "Month",
        int limit = 50, CancellationToken cancellationToken = default);

    Task<UserRankResponse> GetMyRankAsync(string type = "Events", string timeRange = "AllTime",
        CancellationToken cancellationToken = default);

    Task<LeaderboardResponse> GetTeamLeaderboardAsync(string type = "Events", string timeRange = "Month",
        int limit = 50, CancellationToken cancellationToken = default);
}
