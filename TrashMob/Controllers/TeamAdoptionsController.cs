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
    /// Controller for team adoption operations (team-centric view).
    /// </summary>
    [Route("api/teams/{teamId}/adoptions")]
    public class TeamAdoptionsController(
        ITeamAdoptionManager adoptionManager,
        ITeamAdoptionEventManager adoptionEventManager,
        IKeyedManager<Team> teamManager,
        ITeamMemberManager teamMemberManager)
        : SecureController
    {

        /// <summary>
        /// Gets all adoption applications for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTeamAdoptions(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            // Only team members can view team's adoptions
            var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
            if (!isMember)
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetByTeamIdAsync(teamId, cancellationToken);
            return Ok(adoptions);
        }

        /// <summary>
        /// Submits an adoption application for a team. Only team leads can submit.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="request">The adoption application request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoption), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SubmitApplication(
            Guid teamId,
            [FromBody] SubmitAdoptionRequest request,
            CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            // Only team leads can submit applications
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await adoptionManager.SubmitApplicationAsync(
                teamId,
                request.AdoptableAreaId,
                request.ApplicationNotes,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return CreatedAtAction(nameof(GetTeamAdoptions), new { teamId }, result.Data);
        }

        /// <summary>
        /// Gets active adoptions for a team that can have events linked to them.
        /// Useful for event creation dropdown.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("active")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetActiveAdoptions(Guid teamId, CancellationToken cancellationToken)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            // Only team members can view team's active adoptions
            var isMember = await teamMemberManager.IsMemberAsync(teamId, UserId, cancellationToken);
            if (!isMember)
            {
                return Forbid();
            }

            var adoptions = await adoptionEventManager.GetActiveAdoptionsForTeamAsync(teamId, cancellationToken);
            return Ok(adoptions);
        }
    }

    /// <summary>
    /// Request model for submitting an adoption application.
    /// </summary>
    public class SubmitAdoptionRequest
    {
        /// <summary>
        /// Gets or sets the adoptable area ID.
        /// </summary>
        public Guid AdoptableAreaId { get; set; }

        /// <summary>
        /// Gets or sets optional notes about the application.
        /// </summary>
        public string ApplicationNotes { get; set; }
    }
}
