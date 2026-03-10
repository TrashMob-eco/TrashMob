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
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for teams with server-side pagination and filtering.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams")]
    public class TeamsV2Controller(
        ITeamManager teamManager,
        ITeamMemberManager teamMemberManager,
        IKeyedManager<User> userManager,
        ILogger<TeamsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets a paginated list of public, active teams with optional filtering.
        /// </summary>
        /// <param name="filter">Query parameters for pagination and filtering.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paginated list of teams.</returns>
        /// <response code="200">Returns the paginated team list.</response>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResponse<TeamDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeams(
            [FromQuery] TeamQueryParameters filter,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetTeams requested with Page={Page}, PageSize={PageSize}",
                filter.Page, filter.PageSize);

            var query = teamManager.GetFilteredTeamsQueryable(filter);
            var result = await query.ToPagedAsync(filter, t => t.ToV2Dto(), cancellationToken);

            return Ok(result);
        }

        /// <summary>
        /// Gets a single team by its identifier.
        /// </summary>
        /// <param name="id">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The team details.</returns>
        /// <response code="200">Returns the team.</response>
        /// <response code="404">Team not found or inactive.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetTeam(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetTeam requested for {TeamId}", id);

            var team = await teamManager.GetAsync(id, cancellationToken);

            if (team is null || !team.IsActive)
            {
                return NotFound();
            }

            return Ok(team.ToV2Dto());
        }

        /// <summary>
        /// Gets all teams that the current user is a member of.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the user's teams.</response>
        [HttpGet("my")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyTeams(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetMyTeams User={UserId}", UserId);

            var teams = await teamManager.GetTeamsByUserAsync(UserId, cancellationToken);
            return Ok(teams.Select(t => t.ToV2Dto()));
        }

        /// <summary>
        /// Creates a new team. The creator becomes the first team lead.
        /// </summary>
        /// <param name="team">The team to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Team created.</response>
        /// <response code="400">Team name not available or invalid data.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateTeam([FromBody] TeamDto team, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateTeam Name={Name}", team.Name);

            var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, cancellationToken: cancellationToken);
            if (!isAvailable)
            {
                return BadRequest("A team with this name already exists.");
            }

            var user = await userManager.GetAsync(UserId, cancellationToken);
            if (user is null)
            {
                return BadRequest("User not found.");
            }

            var entity = team.ToEntity();
            entity.Id = Guid.NewGuid();
            entity.IsActive = true;
            entity.CreatedByUserId = UserId;
            entity.CreatedDate = DateTimeOffset.UtcNow;
            entity.LastUpdatedByUserId = UserId;
            entity.LastUpdatedDate = DateTimeOffset.UtcNow;

            var createdTeam = await teamManager.AddAsync(entity, UserId, cancellationToken);

            await teamMemberManager.AddMemberAsync(createdTeam.Id, UserId, isTeamLead: true, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetTeam), new { id = createdTeam.Id }, createdTeam.ToV2Dto());
        }

        /// <summary>
        /// Updates a team. Only team leads can update.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="team">The updated team data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated team.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpPut("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeam(Guid teamId, [FromBody] TeamDto team, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateTeam Team={TeamId}", teamId);

            if (teamId != team.Id)
            {
                return BadRequest("Team ID mismatch.");
            }

            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam is null)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            if (!string.Equals(existingTeam.Name, team.Name, StringComparison.OrdinalIgnoreCase))
            {
                var isAvailable = await teamManager.IsTeamNameAvailableAsync(team.Name, teamId, cancellationToken);
                if (!isAvailable)
                {
                    return BadRequest("A team with this name already exists.");
                }
            }

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
            return Ok(updatedTeam.ToV2Dto());
        }

        /// <summary>
        /// Deactivates a team (soft delete). Only team leads can deactivate.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Team deactivated.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpDelete("{teamId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeactivateTeam(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeactivateTeam Team={TeamId}", teamId);

            var existingTeam = await teamManager.GetAsync(teamId, cancellationToken);
            if (existingTeam is null)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            existingTeam.IsActive = false;
            existingTeam.LastUpdatedByUserId = UserId;
            existingTeam.LastUpdatedDate = DateTimeOffset.UtcNow;

            await teamManager.UpdateAsync(existingTeam, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Checks if a team name is available.
        /// </summary>
        /// <param name="name">The team name to check.</param>
        /// <param name="excludeTeamId">Optional team ID to exclude (for updates).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns true if available, false otherwise.</response>
        [HttpGet("check-name")]
        [ProducesResponseType(typeof(bool), StatusCodes.Status200OK)]
        public async Task<IActionResult> CheckTeamName(
            [FromQuery] string name,
            [FromQuery] Guid? excludeTeamId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CheckTeamName Name={Name}", name);

            if (string.IsNullOrWhiteSpace(name))
            {
                return Ok(false);
            }

            var isAvailable = await teamManager.IsTeamNameAvailableAsync(name, excludeTeamId, cancellationToken);
            return Ok(isAvailable);
        }
    }
}
