namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models;
using TrashMob.Models.Poco;

public class EventAttendeeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRestService
{
    protected override string Controller => "eventattendees";

    public async Task<IEnumerable<DisplayUser>> GetEventAttendeesAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}");
        var eventAttendees =
            await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<DisplayUser>>(requestUri, SerializerOptions,
                cancellationToken);
        return eventAttendees ?? [];
    }

    public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);
        var response = await AuthorizedHttpClient.PostAsync(Controller, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventAttendee.EventId}/{eventAttendee.UserId}");

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<IEnumerable<DisplayUser>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}/leads");
        var eventLeads = await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<DisplayUser>>(requestUri, SerializerOptions, cancellationToken);
        return eventLeads ?? [];
    }

    public async Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}/{userId}/promote");
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EventAttendee>(SerializerOptions, cancellationToken);
        return result!;
    }

    public async Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}/{userId}/demote");
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EventAttendee>(SerializerOptions, cancellationToken);
        return result!;
    }
}