namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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
    /// Controller for team email invitation operations.
    /// Endpoints require team lead privileges.
    /// </summary>
    [Authorize]
    [Route("api/teams/{teamId}/invites")]
    public class TeamInvitesController : SecureController
    {
        private readonly IEmailInviteManager emailInviteManager;
        private readonly ITeamManager teamManager;
        private readonly ITeamMemberManager teamMemberManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamInvitesController"/> class.
        /// </summary>
        /// <param name="emailInviteManager">The email invite manager.</param>
        /// <param name="teamManager">The team manager.</param>
        /// <param name="teamMemberManager">The team member manager.</param>
        public TeamInvitesController(
            IEmailInviteManager emailInviteManager,
            ITeamManager teamManager,
            ITeamMemberManager teamMemberManager)
        {
            this.emailInviteManager = emailInviteManager;
            this.teamManager = teamManager;
            this.teamMemberManager = teamMemberManager;
        }

        /// <summary>
        /// Gets all email invite batches for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches for the team with summary statistics.</returns>
        [HttpGet("batches")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailInviteBatch>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatches(Guid teamId, CancellationToken cancellationToken = default)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await emailInviteManager.GetTeamBatchesAsync(teamId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(GetBatches));

            return Ok(result);
        }

        /// <summary>
        /// Gets a batch with its individual invite details for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch with invites or 404 if not found.</returns>
        [HttpGet("batches/{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid teamId, Guid id, CancellationToken cancellationToken = default)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            var result = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null || result.TeamId != teamId)
            {
                return NotFound();
            }

            TrackEvent(nameof(GetBatch));

            return Ok(result);
        }

        /// <summary>
        /// Creates and sends a new batch of email invites for a team.
        /// </summary>
        /// <param name="teamId">The team ID.</param>
        /// <param name="request">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        [HttpPost("batch")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateBatch(
            Guid teamId,
            [FromBody] CreateEmailInviteBatchRequest request,
            CancellationToken cancellationToken = default)
        {
            var team = await teamManager.GetAsync(teamId, cancellationToken);
            if (team == null || !team.IsActive)
            {
                return NotFound();
            }

            // Check if current user is a team lead
            var isLead = await teamMemberManager.IsTeamLeadAsync(teamId, UserId, cancellationToken);
            if (!isLead)
            {
                return Forbid();
            }

            if (request == null || request.Emails == null || !request.Emails.Any())
            {
                return BadRequest("Email list is required.");
            }

            // Create the batch with team context
            var batch = await emailInviteManager.CreateBatchAsync(
                request.Emails,
                UserId,
                "Team",
                null,
                teamId,
                cancellationToken).ConfigureAwait(false);

            // Process the batch (send emails)
            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken).ConfigureAwait(false);

            TrackEvent(nameof(CreateBatch));

            return CreatedAtAction(nameof(GetBatch), new { teamId, id = result.Id }, result);
        }
    }
}
