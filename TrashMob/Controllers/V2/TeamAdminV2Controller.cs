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
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 admin controller for team management. All endpoints require site admin privileges.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/teams")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class TeamAdminV2Controller(
        ITeamManager teamManager,
        ILogger<TeamAdminV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all teams (including inactive and private) for admin review.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of all teams.</returns>
        /// <response code="200">Returns all teams.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Admin GetAllTeams requested by User={UserId}", UserId);

            var teams = await teamManager.GetAllTeamsAsync(cancellationToken);
            return Ok(teams.Select(t => t.ToV2Dto()));
        }

        /// <summary>
        /// Deletes a team. Admin-only hard delete.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Team deleted.</response>
        /// <response code="404">Team not found.</response>
        [HttpDelete("{teamId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Admin DeleteTeam Team={TeamId} by User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            await teamManager.DeleteAsync(teamId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Reactivates a deactivated team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The reactivated team.</returns>
        /// <response code="200">Team reactivated.</response>
        /// <response code="404">Team not found.</response>
        [HttpPost("{teamId}/reactivate")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactivateTeam(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Admin ReactivateTeam Team={TeamId} by User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            team.IsActive = true;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return Ok(updatedTeam.ToV2Dto());
        }
    }
}
