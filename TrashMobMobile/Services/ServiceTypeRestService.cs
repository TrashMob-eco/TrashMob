namespace TrashMobMobile.Services;

using Newtonsoft.Json;
using TrashMob.Models;

public class ServiceTypeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IServiceTypeRestService
{
    protected override string Controller => "servicetypes";

    public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default)
    {

        using (var response = await AnonymousHttpClient.GetAsync(Controller, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonConvert.DeserializeObject<List<ServiceType>>(responseString);
        }
    }
}