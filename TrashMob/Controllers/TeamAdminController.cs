namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for admin team operations.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/teams")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class TeamAdminController(ITeamManager teamManager) : SecureController
    {
        /// <summary>
        /// Gets all teams including private and inactive ones.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetAllTeamsAsync(cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Hard deletes a team. Admin only.
        /// </summary>
        /// <param name="teamId">The team ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{teamId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            await teamManager.DeleteAsync(teamId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Reactivates a deactivated team. Admin only.
        /// </summary>
        /// <param name="teamId">The team ID to reactivate.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{teamId}/reactivate")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ReactivateTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            team.IsActive = true;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(team, UserId, cancellationToken);
            return Ok(updatedTeam);
        }
    }
}
