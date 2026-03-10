namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco;
using TrashMob.Models.Poco.V2;

public class EventAttendeeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRestService
{
    protected override string Controller => "events";

    public async Task<IEnumerable<DisplayUser>> GetEventAttendeesAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/{eventId}/attendees";
        var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var pagedResponse = JsonConvert.DeserializeObject<PagedResponse<EventAttendeeDto>>(content);
        var dtos = pagedResponse?.Items ?? [];
        return dtos.Select(d => new DisplayUser
        {
            Id = d.UserId,
            UserName = d.UserName,
            ProfilePhotoUrl = d.ProfilePhotoUrl,
        });
    }

    public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var dto = new EventAttendeeDto { UserId = eventAttendee.UserId };
        var content = JsonContent.Create(dto, typeof(EventAttendeeDto), null, SerializerOptions);
        var requestUri = Controller + $"/{eventAttendee.EventId}/attendees";
        var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/{eventAttendee.EventId}/attendees/{eventAttendee.UserId}";

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<IEnumerable<DisplayUser>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/{eventId}/attendees/leads";
        var eventLeads = await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<DisplayUser>>(requestUri, SerializerOptions, cancellationToken);
        return eventLeads ?? [];
    }

    public async Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/{eventId}/attendees/{userId}/promote";
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dto = JsonConvert.DeserializeObject<EventAttendeeDto>(content)!;
        var entity = dto.ToEntity();
        entity.EventId = eventId;
        return entity;
    }

    public async Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller + $"/{eventId}/attendees/{userId}/demote";
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dto = JsonConvert.DeserializeObject<EventAttendeeDto>(content)!;
        var entity = dto.ToEntity();
        entity.EventId = eventId;
        return entity;
    }
}
