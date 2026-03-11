namespace TrashMobMobile.Services;

using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco.V2;

public class EventTypeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventTypeRestService
{
    protected override string Controller => "lookups";

    public async Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default)
    {
        using (var response = await AnonymousHttpClient.GetAsync("lookups/event-types", cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

            var dtos = JsonConvert.DeserializeObject<List<LookupItemDto>>(responseString) ?? [];
            return dtos.Select(d => d.ToEventType()).ToList();
        }
    }
}
