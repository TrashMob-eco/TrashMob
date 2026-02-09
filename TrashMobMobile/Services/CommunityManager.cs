namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMob.Models.Poco;

public class CommunityManager(ICommunityRestService service) : ICommunityManager
{
    private readonly ICommunityRestService communityRestService = service;

    public Task<IEnumerable<Partner>> GetCommunitiesAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default)
    {
        return communityRestService.GetCommunitiesAsync(latitude, longitude, radiusMiles, cancellationToken);
    }

    public Task<Partner> GetCommunityBySlugAsync(string slug, CancellationToken cancellationToken = default)
    {
        return communityRestService.GetCommunityBySlugAsync(slug, cancellationToken);
    }

    public Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default)
    {
        return communityRestService.GetCommunityEventsAsync(slug, upcomingOnly, cancellationToken);
    }

    public Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default)
    {
        return communityRestService.GetCommunityTeamsAsync(slug, radiusMiles, cancellationToken);
    }

    public Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default)
    {
        return communityRestService.GetCommunityStatsAsync(slug, cancellationToken);
    }
}
