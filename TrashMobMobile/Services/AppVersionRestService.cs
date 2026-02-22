namespace TrashMobMobile.Services;

using Newtonsoft.Json;
using TrashMob.Models.Poco;

public class AppVersionRestService(IHttpClientFactory httpClientFactory)
    : RestServiceBase(httpClientFactory), IAppVersionRestService
{
    protected override string Controller => "appversion";

    public async Task<AppVersionInfo?> GetAppVersionAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);
            return JsonConvert.DeserializeObject<AppVersionInfo>(responseString);
        }
        catch (Exception)
        {
            // Graceful failure: if version check fails, allow app to continue
            return null;
        }
    }
}
