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
    /// V2 controller for team email invitation operations. Requires team lead privileges.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/teams/{teamId}/invites")]
    public class TeamInvitesV2Controller(
        IEmailInviteManager emailInviteManager,
        IKeyedManager<Team> teamManager,
        ITeamMemberManager teamMemberManager,
        ILogger<TeamInvitesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all email invite batches for a team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches for the team with summary statistics.</returns>
        /// <response code="200">Returns the batch list.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found or inactive.</response>
        [HttpGet("batches")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailInviteBatchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatches(Guid teamId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetBatches for Team={TeamId} User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            if (!team.IsActive)
            {
                return NotFound("Team is not active.");
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var batches = await emailInviteManager.GetTeamBatchesAsync(teamId, cancellationToken);
            return Ok(batches.Select(b => b.ToV2Dto()));
        }

        /// <summary>
        /// Gets a specific email invite batch with its individual invite details.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="batchId">The batch identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch with invite details.</returns>
        /// <response code="200">Returns the batch.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team or batch not found.</response>
        [HttpGet("batches/{batchId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid teamId, Guid batchId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetBatch Batch={BatchId} for Team={TeamId} User={UserId}", batchId, teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            if (!team.IsActive)
            {
                return NotFound("Team is not active.");
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var batch = await emailInviteManager.GetBatchDetailsAsync(batchId, cancellationToken);
            if (batch is null)
            {
                return NotFound();
            }

            return Ok(batch.ToV2Dto());
        }

        /// <summary>
        /// Creates and sends a new batch of email invites for a team.
        /// </summary>
        /// <param name="teamId">The team identifier.</param>
        /// <param name="request">The batch creation request containing email addresses.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        /// <response code="201">Batch created and sent.</response>
        /// <response code="400">Invalid request data or empty email list.</response>
        /// <response code="403">User is not a team lead.</response>
        /// <response code="404">Team not found or inactive.</response>
        [HttpPost("batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateBatch(
            Guid teamId,
            [FromBody] CreateEmailInviteBatchDto request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateBatch for Team={TeamId} User={UserId}", teamId, UserId);

            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team is null)
            {
                return NotFound();
            }

            if (!team.IsActive)
            {
                return NotFound("Team is not active.");
            }

            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            if (request is null || request.Emails is null || !request.Emails.Any())
            {
                return Problem("Email list is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 CreateBatch: {Count} emails for Team={TeamId}",
                request.Emails.Count(), teamId);

            var batch = await emailInviteManager.CreateBatchAsync(
                request.Emails,
                UserId,
                "Team",
                null,
                teamId,
                cancellationToken);

            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken);

            return CreatedAtAction(nameof(GetBatch), new { teamId, batchId = result.Id }, result.ToV2Dto());
        }
    }
}
