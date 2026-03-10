namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco;
using TrashMob.Models.Poco.V2;

public class EventPartnerLocationServiceRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventPartnerLocationServiceRestService
{
    protected override string Controller => "eventpartnerlocationservices";

    public async Task<PartnerLocation> GetHaulingPartnerLocationAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/hauling/" + eventId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<PartnerLocation>(content)!;
            return result;
        }
    }

    public async Task<IEnumerable<DisplayEventPartnerLocation>> GetEventPartnerLocationsAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<List<DisplayEventPartnerLocation>>(content) ?? [];
            return result;
        }
    }

    public async Task<IEnumerable<DisplayEventPartnerLocationService>> GetEventPartnerLocationServicesAsync(
        Guid eventId, Guid partnerId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId + "/" + partnerId;

        using (var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            var result = JsonConvert.DeserializeObject<List<DisplayEventPartnerLocationService>>(content) ?? [];
            return result;
        }
    }

    public async Task<EventPartnerLocationService> UpdateEventPartnerLocationService(
        EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default)
    {
        var dto = new EventPartnerLocationServiceRequestDto
        {
            EventId = eventPartnerLocationService.EventId,
            PartnerLocationId = eventPartnerLocationService.PartnerLocationId,
            ServiceTypeId = eventPartnerLocationService.ServiceTypeId,
            EventPartnerLocationServiceStatusId = eventPartnerLocationService.EventPartnerLocationServiceStatusId,
        };
        var content = JsonContent.Create(dto, typeof(EventPartnerLocationServiceRequestDto), null,
            SerializerOptions);
        var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        // V2 PUT returns 200 with no body; return the original entity with updated values
        return eventPartnerLocationService;
    }

    public async Task<EventPartnerLocationService> AddEventPartnerLocationService(
        EventPartnerLocationService eventPartnerLocationService, CancellationToken cancellationToken = default)
    {
        var dto = new EventPartnerLocationServiceRequestDto
        {
            EventId = eventPartnerLocationService.EventId,
            PartnerLocationId = eventPartnerLocationService.PartnerLocationId,
            ServiceTypeId = eventPartnerLocationService.ServiceTypeId,
            EventPartnerLocationServiceStatusId = eventPartnerLocationService.EventPartnerLocationServiceStatusId,
        };
        var content = JsonContent.Create(dto, typeof(EventPartnerLocationServiceRequestDto), null,
            SerializerOptions);
        var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();

        // V2 POST returns 201 with no body; return the original entity
        return eventPartnerLocationService;
    }

    public async Task DeleteEventPartnerLocationServiceAsync(EventPartnerLocationService eventPartnerLocationService,
        CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller,
            $"/{eventPartnerLocationService.EventId}/{eventPartnerLocationService.PartnerLocationId}/{eventPartnerLocationService.ServiceTypeId}");

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }
}
