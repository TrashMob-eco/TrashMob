namespace TrashMobMobile.Services;

using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

public class EventPartnerLocationServiceStatusRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory),
    IEventPartnerLocationServiceStatusRestService
{
    protected override string Controller => "lookups";

    public async Task<IEnumerable<EventPartnerLocationServiceStatus>> GetEventPartnerLocationServiceStatusesAsync(
        CancellationToken cancellationToken = default)
    {
        using (var response = await AnonymousHttpClient.GetAsync("lookups/partner-service-statuses", cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var dtos = JsonConvert.DeserializeObject<List<LookupItemDto>>(responseString) ?? [];
            return dtos.Select(d => d.ToEventPartnerLocationServiceStatus()).ToList();
        }
    }
}
