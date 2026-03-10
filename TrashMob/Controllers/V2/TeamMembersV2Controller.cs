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
    /// V2 controller for team membership operations as a nested resource under teams.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/members")]
    public class TeamMembersV2Controller(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        ILogger<TeamMembersV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all members of a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the team members.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamMemberDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMembers(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMembers Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            if (!team.IsPublic)
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return NotFound();
                }

                var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
                if (!isMember)
                {
                    return NotFound();
                }
            }

            var members = await teamMemberManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(members.Select(m => m.ToV2Dto()));
        }

        /// <summary>
        /// Gets all team leads of a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the team leads.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("leads")]
        [ProducesResponseType(typeof(IEnumerable<TeamMemberDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLeads(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetLeads Team={TeamId}", teamId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var leads = await teamMemberManager.GetTeamLeadsAsync(teamId, cancellationToken);
            return Ok(leads.Select(m => m.ToV2Dto()));
        }

        /// <summary>
        /// Joins a public team. Private teams cannot be joined directly.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Joined the team.</response>
        /// <response code="400">Already a member or team is private.</response>
        /// <response code="404">Team not found.</response>
        [HttpPost("join")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinTeam(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 JoinTeam Team={TeamId}, User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var existingMember = await teamMemberManager.GetByTeamAndUserAsync(teamId, UserId, cancellationToken);
            if (existingMember is not null)
            {
                return BadRequest("You are already a member of this team.");
            }

            if (!team.IsPublic)
            {
                return BadRequest("This is a private team. You must be invited by a team lead.");
            }

            var member = await teamMemberManager.AddMemberAsync(teamId, UserId, isTeamLead: false, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetMembers), new { teamId }, member.ToV2Dto());
        }

        /// <summary>
        /// Removes a member from a team. Team leads can remove any member, or a member can leave.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Member removed.</response>
        /// <response code="400">Cannot remove the last team lead.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Team or member not found.</response>
        [HttpDelete("{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RemoveMember Team={TeamId}, User={UserId}", teamId, userId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            var member = await teamMemberManager.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member is null)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (userId != UserId && !isLead)
            {
                return Forbid();
            }

            if (member.IsTeamLead)
            {
                var leadCount = await teamMemberManager.GetTeamLeadCountAsync(teamId, cancellationToken);
                if (leadCount <= 1)
                {
                    return BadRequest("Cannot remove the last team lead. Promote another member to lead first or deactivate the team.");
                }
            }

            await teamMemberManager.RemoveMemberAsync(teamId, userId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Promotes a member to team lead. Only existing team leads can promote.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to promote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated team member.</response>
        /// <response code="400">User is already a team lead.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or member not found.</response>
        [HttpPut("{userId}/promote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PromoteToLead(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 PromoteToLead Team={TeamId}, User={UserId}", teamId, userId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var member = await teamMemberManager.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member is null)
            {
                return NotFound("User is not a member of this team.");
            }

            if (member.IsTeamLead)
            {
                return BadRequest("User is already a team lead.");
            }

            var updatedMember = await teamMemberManager.PromoteToLeadAsync(teamId, userId, UserId, cancellationToken);
            return Ok(updatedMember.ToV2Dto());
        }

        /// <summary>
        /// Demotes a team lead to regular member. Only existing team leads can demote.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to demote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated team member.</response>
        /// <response code="400">Cannot demote the last team lead.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or member not found.</response>
        [HttpPut("{userId}/demote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMemberDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DemoteFromLead(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DemoteFromLead Team={TeamId}, User={UserId}", teamId, userId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var member = await teamMemberManager.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (member is null)
            {
                return NotFound("User is not a member of this team.");
            }

            if (!member.IsTeamLead)
            {
                return BadRequest("User is not a team lead.");
            }

            var leadCount = await teamMemberManager.GetTeamLeadCountAsync(teamId, cancellationToken);
            if (leadCount <= 1)
            {
                return BadRequest("Cannot demote the last team lead. Promote another member to lead first.");
            }

            var updatedMember = await teamMemberManager.DemoteFromLeadAsync(teamId, userId, UserId, cancellationToken);
            return Ok(updatedMember.ToV2Dto());
        }
    }
}
