namespace TrashMobMobile.Services;

using TrashMob.Models;

public class TeamManager(ITeamRestService service) : ITeamManager
{
    private readonly ITeamRestService teamRestService = service;

    public Task<IEnumerable<Team>> GetPublicTeamsAsync(double? latitude = null, double? longitude = null, double? radiusMiles = null, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetPublicTeamsAsync(latitude, longitude, radiusMiles, cancellationToken);
    }

    public Task<Team> GetTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetTeamAsync(teamId, cancellationToken);
    }

    public Task<IEnumerable<Team>> GetMyTeamsAsync(CancellationToken cancellationToken = default)
    {
        return teamRestService.GetMyTeamsAsync(cancellationToken);
    }

    public Task<IEnumerable<TeamMember>> GetTeamMembersAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetTeamMembersAsync(teamId, cancellationToken);
    }

    public Task<IEnumerable<TeamMember>> GetTeamLeadsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetTeamLeadsAsync(teamId, cancellationToken);
    }

    public Task<TeamMember> JoinTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.JoinTeamAsync(teamId, cancellationToken);
    }

    public Task<IEnumerable<Event>> GetUpcomingTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetUpcomingTeamEventsAsync(teamId, cancellationToken);
    }

    public Task<IEnumerable<Event>> GetPastTeamEventsAsync(Guid teamId, CancellationToken cancellationToken = default)
    {
        return teamRestService.GetPastTeamEventsAsync(teamId, cancellationToken);
    }
}
