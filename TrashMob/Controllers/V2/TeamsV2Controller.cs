namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
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
        ILogger<TeamsV2Controller> logger) : ControllerBase
    {
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
    }
}
