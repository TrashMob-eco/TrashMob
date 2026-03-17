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
    /// V2 controller for team adoption operations. Requires team membership for access.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/adoptions")]
    public class TeamAdoptionsV2Controller(
        ITeamAdoptionManager adoptionManager,
        ITeamAdoptionEventManager adoptionEventManager,
        IKeyedManager<Team> teamManager,
        ITeamMemberManager teamMemberManager,
        ILogger<TeamAdoptionsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all adoption applications for a team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of the team's adoptions.</returns>
        /// <response code="200">Returns the team's adoptions.</response>
        /// <response code="403">User is not a team member.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamAdoptions(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetTeamAdoptions Team={TeamId} User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
            if (!isMember)
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Submits an adoption application for a team. Only team leads can submit.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="request">The adoption application request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created adoption application.</returns>
        /// <response code="201">Adoption application submitted.</response>
        /// <response code="400">Invalid request or application already exists.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoptionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitApplication(
            Guid teamId,
            [FromBody] SubmitAdoptionRequestDto request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SubmitApplication Team={TeamId} Area={AreaId} User={UserId}",
                teamId, request.AdoptableAreaId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await adoptionManager.SubmitApplicationAsync(
                teamId,
                request.AdoptableAreaId,
                request.ApplicationNotes ?? string.Empty,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(
                nameof(GetTeamAdoptions),
                new { teamId },
                result.Data.ToV2Dto());
        }

        /// <summary>
        /// Gets active adoptions for a team that can have events linked to them.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of the team's active adoptions.</returns>
        /// <response code="200">Returns the team's active adoptions.</response>
        /// <response code="403">User is not a team member.</response>
        /// <response code="404">Team not found.</response>
        [HttpGet("active")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoptionDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveAdoptions(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetActiveAdoptions Team={TeamId} User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
            if (!isMember)
            {
                return Forbid();
            }

            var adoptions = await adoptionEventManager.GetActiveAdoptionsForTeamAsync(teamId, cancellationToken);
            return Ok(adoptions.Select(a => a.ToV2Dto()));
        }
    }
}
