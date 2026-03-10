namespace TrashMobMobile.Services;

using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

public class ServiceTypeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IServiceTypeRestService
{
    protected override string Controller => "lookups";

    public async Task<IEnumerable<ServiceType>> GetServiceTypesAsync(CancellationToken cancellationToken = default)
    {
        using (var response = await AnonymousHttpClient.GetAsync("lookups/service-types", cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var dtos = JsonConvert.DeserializeObject<List<LookupItemDto>>(responseString) ?? [];
            return dtos.Select(d => d.ToServiceType()).ToList();
        }
    }
}
