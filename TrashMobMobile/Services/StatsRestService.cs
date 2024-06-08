namespace TrashMobMobile.Data;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models.Poco;
using TrashMobMobile.Services;

public class StatsRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IStatsRestService
{
    protected override string Controller => "stats";

    public async Task<Stats> GetStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<Stats>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }

    public async Task<Stats> GetUserStatsAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var requestUri = Controller + "/" + userId;

            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<Stats>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}