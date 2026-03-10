namespace TrashMobMobile.Services;

using System.Net.Http.Json;
using TrashMob.Models;
using TrashMob.Models.Poco;

public class EventAttendeeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRestService
{
    protected override string Controller => "events/";
    protected override bool UseV2 => true;

    public async Task<IEnumerable<DisplayUser>> GetEventAttendeesAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = eventId + "/attendees";
        var eventAttendees =
            await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<DisplayUser>>(requestUri, SerializerOptions,
                cancellationToken);
        return eventAttendees ?? [];
    }

    public async Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var requestUri = eventAttendee.EventId + "/attendees";
        var content = JsonContent.Create(eventAttendee, typeof(EventAttendee), null, SerializerOptions);
        var response = await AuthorizedHttpClient.PostAsync(requestUri, content, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
    {
        var requestUri = eventAttendee.EventId + "/attendees/" + eventAttendee.UserId;

        using (var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken))
        {
            response.EnsureSuccessStatusCode();
        }
    }

    public async Task<IEnumerable<DisplayUser>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = eventId + "/attendees/leads";
        var eventLeads = await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<DisplayUser>>(requestUri, SerializerOptions, cancellationToken);
        return eventLeads ?? [];
    }

    public async Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = eventId + "/attendees/" + userId + "/promote";
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EventAttendee>(SerializerOptions, cancellationToken);
        return result!;
    }

    public async Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
    {
        var requestUri = eventId + "/attendees/" + userId + "/demote";
        var response = await AuthorizedHttpClient.PutAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<EventAttendee>(SerializerOptions, cancellationToken);
        return result!;
    }
}
