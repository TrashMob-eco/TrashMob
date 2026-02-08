namespace TrashMobMobile.Services;

using System.Globalization;
using Newtonsoft.Json;
using TrashMob.Models;

public class TeamRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), ITeamRestService
{
    protected override string Controller => "teams";

    public async Task<IEnumerable<Team>> GetPublicTeamsAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default)
    {
        var requestUri = Controller;
        var queryParams = new List<string>();

        if (latitude.HasValue)
        {
            queryParams.Add($"latitude={latitude.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (longitude.HasValue)
        {
            queryParams.Add($"longitude={longitude.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (radiusMiles.HasValue)
        {
            queryParams.Add($"radiusMiles={radiusMiles.Value.ToString(CultureInfo.InvariantCulture)}");
        }

        if (queryParams.Count > 0)
        {
            requestUri += "?" + string.Join("&", queryParams);
        }

        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<Team>>(content) ?? [];
    }

    public async Task<Team> GetTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<Team>(content);
    }

    public async Task<IEnumerable<Team>> GetMyTeamsAsync(CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/my";
        using var response = await AuthorizedHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<Team>>(content) ?? [];
    }

    public async Task<IEnumerable<TeamMember>> GetTeamMembersAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/members";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<TeamMember>>(content) ?? [];
    }

    public async Task<IEnumerable<TeamMember>> GetTeamLeadsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/members/leads";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<TeamMember>>(content) ?? [];
    }

    public async Task<TeamMember> JoinTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/members/join";
        using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<TeamMember>(content);
    }

    public async Task<IEnumerable<Event>> GetUpcomingTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/events/upcoming";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<Event>>(content) ?? [];
    }

    public async Task<IEnumerable<Event>> GetPastTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/events/past";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        return JsonConvert.DeserializeObject<List<Event>>(content) ?? [];
    }

    public async Task LinkEventAsync(Guid teamId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/events/{eventId}";
        using var response = await AuthorizedHttpClient.PostAsync(requestUri, null, cancellationToken);
        response.EnsureSuccessStatusCode();
    }

    public async Task UnlinkEventAsync(Guid teamId, Guid eventId, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{teamId}/events/{eventId}";
        using var response = await AuthorizedHttpClient.DeleteAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
    }
}
