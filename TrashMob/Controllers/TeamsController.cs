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
    /// Controller for team operations.
    /// </summary>
    [Route("api/teams")]
    public class TeamsController(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IKeyedManager<User> userManager)
        : SecureController
    {
        /// <summary>
        /// Gets all public teams.
        /// </summary>
        /// <param name="latitude">Optional latitude for location filtering.</param>
        /// <param name="longitude">Optional longitude for location filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles for location filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPublicTeams(
            [FromQuery] double? latitude,
            [FromQuery] double? longitude,
            [FromQuery] double? radiusMiles,
            CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetPublicTeamsAsync(latitude, longitude, radiusMiles, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Gets a team by ID.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{teamId}")]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null)
            {
                return NotFound();
            }

            // Private teams can only be viewed by members
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

            return Ok(team);
        }

        /// <summary>
        /// Gets all teams that the current user is a member of.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTeams(CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetTeamsByUserAsync(UserId, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Gets all teams that the current user leads.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("my/leading")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Team>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTeamsILead(CancellationToken cancellationToken)
        {
            var teams = await teamManager.GetTeamsUserLeadsAsync(UserId, cancellationToken);
            return Ok(teams);
        }

        /// <summary>
        /// Checks if a team name is available.
        /// </summary>
        /// <param name="name">The team name to check.</param>
        /// <param name="excludeTeamId">Optional team ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("check-name")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckTeamName(
            [FromQuery] string name,
            [FromQuery] Guid? excludeTeamId,
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(false);
            }

            var isAvailable = await teamManager.IsTeamNameAvailableAsync(name, excludeTeamId, cancellationToken);
            return Ok(isAvailable);
        }

        /// <summary>
        /// Creates a new team. The creator becomes the first team lead.
        /// </summary>
        /// <param name="team">The team to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam([FromBody] Team team, CancellationToken cancellationToken)
        {
            // Check if team name is available
            var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, cancellationToken: cancellationToken);
            if (!isAvailable)
            {
                return BadRequest("A team with this name already exists.");
            }

            // Verify the user is an adult (18+) to create a team
            var user = await userManager.GetAsync(UserId, cancellationToken);
            if (user == null)
            {
                return BadRequest("User not found.");
            }

            // Set audit fields
            team.Id = Guid.NewGuid();
            team.IsActive = true;
            team.CreatedByUserId = UserId;
            team.CreatedDate = DateTimeOffset.UtcNow;
            team.LastUpdatedByUserId = UserId;
            team.LastUpdatedDate = DateTimeOffset.UtcNow;

            var createdTeam = await teamManager.AddAsync(team, UserId, cancellationToken);

            // Add creator as team lead
            await teamMemberManager.AddMemberAsync(createdTeam.Id, UserId, isTeamLead: true, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetTeam), new { teamId = createdTeam.Id }, createdTeam);
        }

        /// <summary>
        /// Updates a team. Only team leads can update a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="team">The updated team data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Team), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] Team team, CancellationToken cancellationToken)
        {
            if (teamId != team.Id)
            {
                return BadRequest("Team ID mismatch.");
            }

            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Check if name is being changed and is available
            if (!string.Equals(existingTeam.Name, team.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, teamId, cancellationToken);
                if (!isAvailable)
                {
                    return BadRequest("A team with this name already exists.");
                }
            }

            // Update fields
            existingTeam.Name = team.Name;
            existingTeam.Description = team.Description;
            existingTeam.LogoUrl = team.LogoUrl;
            existingTeam.IsPublic = team.IsPublic;
            existingTeam.RequiresApproval = team.RequiresApproval;
            existingTeam.Latitude = team.Latitude;
            existingTeam.Longitude = team.Longitude;
            existingTeam.City = team.City;
            existingTeam.Region = team.Region;
            existingTeam.Country = team.Country;
            existingTeam.PostalCode = team.PostalCode;
            existingTeam.LastUpdatedByUserId = UserId;
            existingTeam.LastUpdatedDate = DateTimeOffset.UtcNow;

            var updatedTeam = await teamManager.UpdateAsync(existingTeam, UserId, cancellationToken);
            return Ok(updatedTeam);
        }

        /// <summary>
        /// Deactivates a team (soft delete). Only team leads can deactivate a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateTeam(Guid teamId, CancellationToken cancellationToken)
        {
            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam == null)
            {
                return NotFound();
            }

            // Check if user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            // Soft delete
            existingTeam.IsActive = false;
            existingTeam.LastUpdatedByUserId = UserId;
            existingTeam.LastUpdatedDate = DateTimeOffset.UtcNow;

            await teamManager.UpdateAsync(existingTeam, UserId, cancellationToken);
            return NoContent();
        }
    }
}
