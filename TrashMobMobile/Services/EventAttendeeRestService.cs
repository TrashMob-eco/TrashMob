namespace TrashMobMobile.Services;

using System.Diagnostics;
using System.Net.Http.Json;
using TrashMob.Models;

public class EventAttendeeRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), IEventAttendeeRestService
{
    protected override string Controller => "eventattendees";

    public async Task<IEnumerable<EventAttendee>> GetEventAttendeesAsync(Guid eventId,
        CancellationToken cancellationToken = default)
    {
        var requestUri = string.Concat(Controller, $"/{eventId}");
        var eventAttendees =
            await AuthorizedHttpClient.GetFromJsonAsync<IEnumerable<EventAttendee>>(requestUri, SerializerOptions,
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
}