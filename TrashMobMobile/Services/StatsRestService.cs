namespace TrashMobMobile.Services;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models.Poco;

public class StatsRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IStatsRestService
{
    protected override string Controller => "stats";

    public async Task<Stats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<Stats>(responseString);
        }
    }

    public async Task<Stats> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + userId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<Stats>(responseString);
        }
    }
}