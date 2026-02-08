namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using System.Text.Json;
using TrashMobMobile.Models;

public class AchievementRestService(IHttpClientFactory httpClientFactory)
    : RestServiceBase(httpClientFactory), IAchievementRestService
{
    protected override string Controller => "achievements";

    public async Task<UserAchievementsResponse> GetMyAchievementsAsync(
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/my";

        using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        return JsonSerializer.Deserialize<UserAchievementsResponse>(content, SerializerOptions)
               ?? new UserAchievementsResponse();
    }

    public async Task MarkAsReadAsync(IEnumerable<int> achievementTypeIds,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/my/mark-read";

        using var response = await AuthorizedHttpClient.PostAsJsonAsync(requestUri, achievementTypeIds, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
