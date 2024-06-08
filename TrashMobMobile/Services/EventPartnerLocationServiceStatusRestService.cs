namespace TrashMobMobile.Data;

using System.Diagnostics;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMobMobile.Services;

public class EventPartnerLocationServiceStatusRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory),
    IEventPartnerLocationServiceStatusRestService
{
    protected override string Controller => "eventpartnerlocationservicestatuses";

    public async Task<IEnumerable<EventPartnerLocationServiceStatus>> GetEventPartnerLocationServiceStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

                return JsonConvert.DeserializeObject<List<EventPartnerLocationServiceStatus>>(responseString);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine(@"\tERROR {0}", ex.Message);
            throw;
        }
    }
}