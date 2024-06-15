namespace TrashMobMobile.Services;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models;

public class WaiverRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IWaiverRestService
{
    protected override string Controller => "waivers";

    public async Task<Waiver> GetWaiver(string waiverName, CancellationToken cancellationToken)
    {
        try
        {
            var requestUri = Controller + "/" + waiverName;
            using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<Waiver>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}