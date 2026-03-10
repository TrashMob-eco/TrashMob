namespace TrashMobMobile.Services;

using System.Globalization;
using Newtonsoft.Json;
using TrashMob.Models;
using TrashMob.Models.Extensions.V2;
using TrashMob.Models.Poco;
using TrashMob.Models.Poco.V2;

public class CommunityRestService(IHttpClientFactory httpClientFactory) : RestServiceBase(httpClientFactory), ICommunityRestService
{
    protected override string Controller => "communities";

    public async Task<IEnumerable<Partner>> GetCommunitiesAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default)
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
        var dtos = JsonConvert.DeserializeObject<List<PartnerDto>>(content) ?? [];
        return dtos.Select(d => d.ToEntity()).ToList();
    }

    public async Task<Partner> GetCommunityBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{slug}";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dto = JsonConvert.DeserializeObject<PartnerDto>(content)!;
        return dto.ToEntity();
    }

    public async Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{slug}/events?upcomingOnly={upcomingOnly}";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonConvert.DeserializeObject<List<EventDto>>(content) ?? [];
        return dtos.Select(d => d.ToEntity()).ToList();
    }

    public async Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{slug}/teams?radiusMiles={radiusMiles.ToString(CultureInfo.InvariantCulture)}";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dtos = JsonConvert.DeserializeObject<List<TeamDto>>(content) ?? [];
        return dtos.Select(d => d.ToEntity()).ToList();
    }

    public async Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default)
    {
        var requestUri = $"{Controller}/{slug}/stats";
        using var response = await AnonymousHttpClient.GetAsync(requestUri, cancellationToken);
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var dto = JsonConvert.DeserializeObject<StatsDto>(content)!;
        return dto.ToEntity();
    }
}
