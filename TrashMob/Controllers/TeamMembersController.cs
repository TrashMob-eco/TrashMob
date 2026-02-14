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
    /// Controller for team membership operations.
    /// </summary>
    [Route("api/teams/{teamId}/members")]
    public class TeamMembersController(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager)
        : SecureController
    {
        /// <summary>
        /// Gets all members of a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TeamMember>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetMembers(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Private teams: only members can view the member list
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

            var members = await teamMemberManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(members);
        }

        /// <summary>
        /// Gets all team leads of a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("leads")]
        [ProducesResponseType(typeof(IEnumerable<TeamMember>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetLeads(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            var leads = await teamMemberManager.GetTeamLeadsAsync(teamId, cancellationToken);
            return Ok(leads);
        }

        /// <summary>
        /// Requests to join a team (for public teams) or is added directly by a lead (for private teams).
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("join")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMember), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> JoinTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if already a member
            var existingMember = await teamMemberManager.GetByTeamAndUserAsync(teamId, UserId, cancellationToken);
            if (existingMember is not null)
            {
                return BadRequest("You are already a member of this team.");
            }

            // Private teams cannot be joined directly
            if (!team.IsPublic)
            {
                return BadRequest("This is a private team. You must be invited by a team lead.");
            }

            // For public teams that don't require approval, add immediately
            if (!team.RequiresApproval)
            {
                var member = await teamMemberManager.AddMemberAsync(teamId, UserId, isTeamLead: false, UserId, cancellationToken);
                return CreatedAtAction(nameof(GetMembers), new { teamId }, member);
            }

            // For teams requiring approval, this would create a join request
            // For now, we'll add them directly (join request workflow to be implemented)
            var newMember = await teamMemberManager.AddMemberAsync(teamId, UserId, isTeamLead: false, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetMembers), new { teamId }, newMember);
        }

        /// <summary>
        /// Adds a member to a team. Only team leads can add members.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMember), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddMember(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Check if user is already a member
            var existingMember = await teamMemberManager.GetByTeamAndUserAsync(teamId, userId, cancellationToken);
            if (existingMember is not null)
            {
                return BadRequest("User is already a member of this team.");
            }

            var member = await teamMemberManager.AddMemberAsync(teamId, userId, isTeamLead: false, UserId, cancellationToken);
            return CreatedAtAction(nameof(GetMembers), new { teamId }, member);
        }

        /// <summary>
        /// Removes a member from a team. Team leads can remove any member, or a member can leave.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemoveMember(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
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

            // User can remove themselves, or a team lead can remove others
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (userId != UserId && !isLead)
            {
                return Forbid();
            }

            // Check if this is the last lead trying to leave
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
        [HttpPut("{userId}/promote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMember), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PromoteToLead(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
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
            return Ok(updatedMember);
        }

        /// <summary>
        /// Demotes a team lead to regular member. Only existing team leads can demote.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="userId">The user ID to demote.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{userId}/demote")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamMember), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DemoteFromLead(Guid teamId, Guid userId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
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

            // Cannot demote the last lead
            var leadCount = await teamMemberManager.GetTeamLeadCountAsync(teamId, cancellationToken);
            if (leadCount <= 1)
            {
                return BadRequest("Cannot demote the last team lead. Promote another member to lead first.");
            }

            var updatedMember = await teamMemberManager.DemoteFromLeadAsync(teamId, userId, UserId, cancellationToken);
            return Ok(updatedMember);
        }
    }
}
