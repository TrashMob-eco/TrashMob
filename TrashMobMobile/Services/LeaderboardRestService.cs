namespace TrashMobMobile.Services;

using System.Text.Json;
using TrashMobMobile.Models;

public class LeaderboardRestService(IHttpClientFactory httpClientFactory)
    : RestServiceBase(httpClientFactory), ILeaderboardRestService
{
    protected override string Controller => "leaderboards";

    public async Task<LeaderboardResponse> GetLeaderboardAsync(string type = "Events",
        string timeRange = "Month", int limit = 50, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"?type={type}&timeRange={timeRange}&limit={limit}";

        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<LeaderboardResponse>(content, SerializerOptions)
               ?? new LeaderboardResponse();
    }

    public async Task<UserRankResponse> GetMyRankAsync(string type = "Events",
        string timeRange = "AllTime", CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/my-rank?type={type}&timeRange={timeRange}";

        using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<UserRankResponse>(content, SerializerOptions)
               ?? new UserRankResponse();
    }

    public async Task<LeaderboardResponse> GetTeamLeaderboardAsync(string type = "Events",
        string timeRange = "Month", int limit = 50, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/teams?type={type}&timeRange={timeRange}&limit={limit}";

        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<LeaderboardResponse>(content, SerializerOptions)
               ?? new LeaderboardResponse();
    }
}
