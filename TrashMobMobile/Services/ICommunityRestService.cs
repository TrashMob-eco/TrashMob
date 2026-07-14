namespace TrashMobMobile.Services;

using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMob.Models.Poco.V2;

public interface ICommunityRestService
{
    Task<IEnumerable<Partner>> GetCommunitiesAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default);

    Task<Partner> GetCommunityBySlugAsync(string slug, CancellationToken cancellationToken = default);

    Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default);

    Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default);

    Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default);

    /// <summary>
    /// Returns the community whose bounds contain the given GPS point, or null when
    /// no enabled community matches. Backing endpoint returns 404 when no match — the
    /// implementation translates that to a null result. See Project 65 Phase 3.
    /// </summary>
    Task<CommunityLocationMatchDto?> GetCommunityAtLocationAsync(double latitude, double longitude,
        CancellationToken cancellationToken = default);
}
