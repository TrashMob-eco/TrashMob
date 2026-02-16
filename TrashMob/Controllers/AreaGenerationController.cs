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
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Services;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for AI-powered area generation within a community.
    /// </summary>
    [Route("api/communities/{partnerId}/areas/generate")]
    public class AreaGenerationController(
        IAreaGenerationBatchManager batchManager,
        IKeyedManager<Partner> partnerManager,
        IAreaGenerationQueue queue)
        : SecureController
    {
        /// <summary>
        /// Starts a new area generation batch.
        /// </summary>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaGenerationBatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StartGeneration(
            Guid partnerId,
            [FromBody] AreaGenerationRequest request,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (string.IsNullOrWhiteSpace(request.Category))
            {
                return BadRequest("Category is required.");
            }

            // Check for an already-running batch
            var active = await batchManager.GetActiveByPartnerAsync(partnerId, cancellationToken);
            if (active != null)
            {
                return Conflict("A generation batch is already running for this community.");
            }

            var batch = new AreaGenerationBatch
            {
                Id = Guid.NewGuid(),
                PartnerId = partnerId,
                Category = request.Category,
                Status = "Queued",
                BoundsNorth = request.BoundsNorth,
                BoundsSouth = request.BoundsSouth,
                BoundsEast = request.BoundsEast,
                BoundsWest = request.BoundsWest,
                CreatedByUserId = UserId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedByUserId = UserId,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            var created = await batchManager.AddAsync(batch, UserId, cancellationToken);

            await queue.QueueAsync(created.Id, cancellationToken);

            return CreatedAtAction(nameof(GetBatch), new { partnerId, batchId = created.Id }, created);
        }

        /// <summary>
        /// Gets the active (running) batch status for polling.
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(typeof(AreaGenerationBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus(Guid partnerId, CancellationToken cancellationToken)
        {
            var active = await batchManager.GetActiveByPartnerAsync(partnerId, cancellationToken);
            if (active is null)
            {
                return NotFound();
            }

            return Ok(active);
        }

        /// <summary>
        /// Lists all generation batches for a community.
        /// </summary>
        [HttpGet("batches")]
        [ProducesResponseType(typeof(IEnumerable<AreaGenerationBatch>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBatches(Guid partnerId, CancellationToken cancellationToken)
        {
            var batches = await batchManager.GetByPartnerAsync(partnerId, cancellationToken);
            return Ok(batches);
        }

        /// <summary>
        /// Gets a specific batch.
        /// </summary>
        [HttpGet("batches/{batchId}")]
        [ProducesResponseType(typeof(AreaGenerationBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid partnerId, Guid batchId, CancellationToken cancellationToken)
        {
            var batch = await batchManager.GetAsync(batchId, cancellationToken);
            if (batch is null || batch.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(batch);
        }

        /// <summary>
        /// Cancels a running generation batch.
        /// </summary>
        [HttpDelete("batches/{batchId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBatch(
            Guid partnerId,
            Guid batchId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var batch = await batchManager.GetAsync(batchId, cancellationToken);
            if (batch is null || batch.PartnerId != partnerId)
            {
                return NotFound();
            }

            batch.Status = "Cancelled";
            batch.CompletedDate = DateTimeOffset.UtcNow;
            batch.LastUpdatedByUserId = UserId;
            batch.LastUpdatedDate = DateTimeOffset.UtcNow;
            await batchManager.UpdateAsync(batch, cancellationToken);

            return NoContent();
        }
    }
}
