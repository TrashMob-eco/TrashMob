namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMob.Models.Poco;

public interface ICommunityManager
{
    Task<IEnumerable<Partner>> GetCommunitiesAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default);

    Task<Partner> GetCommunityBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default);

    Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default);
}
