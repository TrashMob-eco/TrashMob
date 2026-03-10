namespace TrashMobMobile.Services;

using System.Globalization;
using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco;
using TrashMob.Models.Poco.V2;
using TrashMobMobile.Models;

public class MobEventRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IMobEventRestService
{
    protected override string Controller => "events";

    public async Task<PaginatedList<Event>> GetFilteredEventsAsync(EventFilter filter, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(filter, typeof(EventFilter), null, SerializerOptions);
        var requestUri = $"{Controller}/pagedfilteredevents";
        var response = await AnonymousHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtoResult = JsonConvert.DeserializeObject<PaginatedList<EventDto>>(returnContent) ?? new();
        var result = new PaginatedList<Event>();
        result.AddRange(dtoResult.Select(d => d.ToEntity()));
        return result;
    }

    public async Task<PaginatedList<Event>> GetUserEventsAsync(EventFilter filter, Guid userId, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(filter, typeof(EventFilter), null, SerializerOptions);
        var requestUri = $"{Controller}/pageduserevents/{userId}";
        var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtoResult = JsonConvert.DeserializeObject<PaginatedList<EventDto>>(returnContent) ?? new();
        var result = new PaginatedList<Event>();
        result.AddRange(dtoResult.Select(d => d.ToEntity()));
        return result;
    }

    public async Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly,
        CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/userevents/{userId}/{showFutureEventsOnly}";
        var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonConvert.DeserializeObject<List<EventDto>>(content) ?? [];

        return dtos.Select(d => d.ToEntity());
    }

    public async Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + "/" + eventId;
        var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<EventDto>(content)!.ToEntity();
    }

    public async Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
    {
        var dto = mobEvent.ToV2Dto();
        var content = JsonContent.Create(dto, typeof(EventDto), null, SerializerOptions);
        var response = await AuthorizedHttpClient.PutAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<EventDto>(returnContent)!.ToEntity();
    }

    public async Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
    {
        var dto = mobEvent.ToV2Dto();
        var content = JsonContent.Create(dto, typeof(EventDto), null, SerializerOptions);
        var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();
        var returnContent = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<EventDto>(returnContent)!.ToEntity();
    }

    public async Task DeleteEventAsync(EventCancellationRequest cancelEvent,
        CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(cancelEvent, typeof(EventCancellationRequest), null, SerializerOptions);
        var httpRequestMessage = new HttpRequestMessage
        {
            Method = HttpMethod.Delete,
            Content = content,
            RequestUri = AuthorizedHttpClient.BaseAddress,
        };

        var response = await AuthorizedHttpClient.SendAsync(httpRequestMessage, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/eventsuserisattending/{userId}";
        var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonConvert.DeserializeObject<List<EventDto>>(content) ?? [];

        return dtos.Select(d => d.ToEntity());
    }

    public async Task<IEnumerable<Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
        DateTimeOffset endDate, CancellationToken cancellationToken = default)
    {
        var startDateTime = startDate.ToString();
        var endDateTime = endDate.ToString();

        // Convert the DateTime string into the correct Format if the Culture is not Invariant
        if (CultureInfo.CurrentCulture != CultureInfo.InvariantCulture)
        {
            startDateTime = startDate.ToString(@"yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US"));
            endDateTime = endDate.ToString(@"yyyy/MM/dd hh:mm:ss tt", new CultureInfo("en-US"));
        }

        var requestUri = Controller + "/locationsbytimerange?startTime=" + startDateTime + "&endTime=" + endDateTime;

        using (var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);

            if (string.IsNullOrEmpty(content))
            {
                return [];
            }

            return JsonConvert.DeserializeObject<IEnumerable<Location>>(content) ?? [];
        }
    }
}
