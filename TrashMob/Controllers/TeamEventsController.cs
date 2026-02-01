namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Controller for team event associations.
    /// </summary>
    [Route("api/teams/{teamId}/events")]
    public class TeamEventsController(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IKeyedRepository<TeamEvent> teamEventRepository,
        IKeyedManager<Event> eventManager)
        : SecureController
    {
        /// <summary>
        /// Gets all events associated with a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamEvents(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Private teams: only members can view events
            if (!team.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
                }
            }

            var teamEvents = await teamEventRepository.Get()
                .Where(te => te.TeamId == teamId)
                .Include(te => te.Event)
                .Select(te => te.Event)
                .ToListAsync(cancellationToken);

            return Ok(teamEvents);
        }

        /// <summary>
        /// Gets upcoming events for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("upcoming")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetUpcomingTeamEvents(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Private teams: only members can view events
            if (!team.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
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

            return Ok(teamEvents);
        }

        /// <summary>
        /// Gets past events for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("past")]
        [ProducesResponseType(typeof(IEnumerable<Event>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPastTeamEvents(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Private teams: only members can view events
            if (!team.IsPublic)
            {
                if (!User.Identity.IsAuthenticated)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
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

            return Ok(teamEvents);
        }

        /// <summary>
        /// Links an event to a team. Only team leads can link events.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="eventId">The event ID to link.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamEvent), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> LinkEventToTeam(Guid teamId, Guid eventId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound("Team not found.");
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Check if event exists
            var eventEntity = await eventManager.GetAsync(eventId, cancellationToken);
            if (eventEntity == null)
            {
                return NotFound("Event not found.");
            }

            // Check if already linked
            var existingLink = await teamEventRepository.Get()
                .FirstOrDefaultAsync(te => te.TeamId == teamId && te.EventId == eventId, cancellationToken);

            if (existingLink != null)
            {
                return BadRequest("Event is already linked to this team.");
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
            return CreatedAtAction(nameof(GetTeamEvents), new { teamId }, created);
        }

        /// <summary>
        /// Unlinks an event from a team. Only team leads can unlink events.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="eventId">The event ID to unlink.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{eventId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UnlinkEventFromTeam(Guid teamId, Guid eventId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound("Team not found.");
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var teamEvent = await teamEventRepository.Get()
                .FirstOrDefaultAsync(te => te.TeamId == teamId && te.EventId == eventId, cancellationToken);

            if (teamEvent == null)
            {
                return NotFound("Event is not linked to this team.");
            }

            await teamEventRepository.DeleteAsync(teamEvent.Id);
            return NoContent();
        }
    }
}
