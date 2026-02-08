namespace TrashMobMobile.Services;

using TrashMob.Models;

public interface ITeamManager
{
    Task<IEnumerable<Team>> GetPublicTeamsAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default);

    Task<Team> GetTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Team>> GetMyTeamsAsync(CancellationToken cancellationToken = default);

    Task<IEnumerable<TeamMember>> GetTeamMembersAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<IEnumerable<TeamMember>> GetTeamLeadsAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<TeamMember> JoinTeamAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Event>> GetUpcomingTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default);

    Task<IEnumerable<Event>> GetPastTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default);
}
