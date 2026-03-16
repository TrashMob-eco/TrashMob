namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// V2 controller for team event associations as a nested resource under teams.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/events")]
    public class TeamEventsV2Controller(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IKeyedRepository<TeamEvent> teamEventRepository,
        IKeyedManager<Event> eventManager,
        ILogger<TeamEventsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets upcoming events for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns upcoming team events.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("upcoming")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUpcomingTeamEvents(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetUpcomingTeamEvents Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            if (!team.IsPublic)
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
                }
            }

            var now = DateTimeOffset.UtcNow;
            var teamEvents = await teamEventRepository.Get()
                .Where(te => te.TeamId == teamId)
                .Include(te => te.Event)
                .Where(te => te.Event.EventDate >= now && te.Event.EventStatusId != (int)EventStatusEnum.Canceled)
                .Select(te => te.Event)
                .OrderBy(e => e.EventDate)
                .ToListAsync(cancellationToken);

            return Ok(teamEvents.Select(e => e.ToV2Dto()));
        }

        /// <summary>
        /// Gets past events for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns past team events.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("past")]
        [ProducesResponseType(typeof(IEnumerable<EventDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPastTeamEvents(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPastTeamEvents Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            if (!team.IsPublic)
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return Problem(detail: $"Team {teamId} not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
                }
            }

            var now = DateTimeOffset.UtcNow;
            var teamEvents = await teamEventRepository.Get()
                .Where(te => te.TeamId == teamId)
                .Include(te => te.Event)
                .Where(te => te.Event.EventDate < now)
                .Select(te => te.Event)
                .OrderByDescending(e => e.EventDate)
                .ToListAsync(cancellationToken);

            return Ok(teamEvents.Select(e => e.ToV2Dto()));
        }

        /// <summary>
        /// Links an event to a team. Only team leads can link events.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="eventId">The event ID to link.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Event linked to team.</response>
        /// <response code="400">Event already linked.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or event not found.</response>
        [HttpPost("{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamEventDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkEventToTeam(Guid teamId, Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 LinkEventToTeam Team={TeamId}, Event={EventId}", teamId, eventId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return Problem(detail: "Team not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var eventEntity = await eventManager.GetAsync(eventId, cancellationToken);
            if (eventEntity is null)
            {
                return Problem(detail: "Event not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            var existingLink = await teamEventRepository.Get()
                .FirstOrDefaultAsync(te => te.TeamId == teamId && te.EventId == eventId, cancellationToken);

            if (existingLink is not null)
            {
                return Problem(detail: "Event is already linked to this team.", statusCode: StatusCodes.Status400BadRequest, title: "Duplicate link");
            }

            var teamEvent = new TeamEvent
            {
                Id = Guid.NewGuid(),
                TeamId = teamId,
                EventId = eventId,
                CreatedByUserId = UserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = UserId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            var created = await teamEventRepository.AddAsync(teamEvent);
            return CreatedAtAction(nameof(GetUpcomingTeamEvents), new { teamId }, created.ToV2Dto());
        }

        /// <summary>
        /// Unlinks an event from a team. Only team leads can unlink events.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="eventId">The event ID to unlink.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Event unlinked.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or link not found.</response>
        [HttpDelete("{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkEventFromTeam(Guid teamId, Guid eventId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UnlinkEventFromTeam Team={TeamId}, Event={EventId}", teamId, eventId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return Problem(detail: "Team not found.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var teamEvent = await teamEventRepository.Get()
                .FirstOrDefaultAsync(te => te.TeamId == teamId && te.EventId == eventId, cancellationToken);

            if (teamEvent is null)
            {
                return Problem(detail: "Event is not linked to this team.", statusCode: StatusCodes.Status404NotFound, title: "Not found");
            }

            await teamEventRepository.DeleteAsync(teamEvent.Id);
            return NoContent();
        }
    }
}
