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
    /// Controller for bulk email invitation operations.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/invites")]
    public class EmailInvitesController(IEmailInviteManager emailInviteManager)
        : SecureController
    {

        /// <summary>
        /// Gets all email invite batches. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all batches with summary statistics.</returns>
        [HttpGet("batches")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailInviteBatch>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBatches(CancellationToken cancellationToken = default)
        {
            var result = await emailInviteManager.GetAdminBatchesAsync(cancellationToken);
            TrackEvent(nameof(GetBatches));

            return Ok(result);
        }

        /// <summary>
        /// Gets a batch with its individual invite details. Admin only.
        /// </summary>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch with invites or 404 if not found.</returns>
        [HttpGet("batches/{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            TrackEvent(nameof(GetBatch));

            return Ok(result);
        }

        /// <summary>
        /// Creates and sends a new batch of email invites. Admin only.
        /// </summary>
        /// <param name="request">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        [HttpPost("batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateBatch(
            [FromBody] CreateEmailInviteBatchRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null || request.Emails is null || !request.Emails.Any())
            {
                return BadRequest("Email list is required.");
            }

            // Create the batch
            var batch = await emailInviteManager.CreateBatchAsync(
                request.Emails,
                UserId,
                "Admin",
                null,
                null,
                cancellationToken);

            // Process the batch (send emails)
            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken);

            TrackEvent(nameof(CreateBatch));

            return CreatedAtAction(nameof(GetBatch), new { id = result.Id }, result);
        }
    }

    /// <summary>
    /// Request model for creating a batch of email invites.
    /// </summary>
    public class CreateEmailInviteBatchRequest
    {
        /// <summary>
        /// Gets or sets the list of email addresses to invite.
        /// </summary>
        public IEnumerable<string> Emails { get; set; }
    }
}
