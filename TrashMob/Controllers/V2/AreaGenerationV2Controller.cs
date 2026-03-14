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
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Services;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for AI-powered area generation within a community.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/areas/generate")]
    public class AreaGenerationV2Controller(
        IAreaGenerationBatchManager batchManager,
        IKeyedManager<Partner> partnerManager,
        IAreaGenerationQueue queue,
        IAuthorizationService authorizationService,
        ILogger<AreaGenerationV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Starts a new area generation batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="request">The generation request with category and optional bounds.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaGenerationBatchDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StartGeneration(
            Guid partnerId,
            [FromBody] AreaGenerationRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 StartGeneration for Partner={PartnerId}, Category={Category}", partnerId, request.Category);

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

            return CreatedAtAction(nameof(GetBatch), new { partnerId, batchId = created.Id }, created.ToV2Dto());
        }

        /// <summary>
        /// Gets the active (running) batch status for polling.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("status")]
        [ProducesResponseType(typeof(AreaGenerationBatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStatus(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetStatus for Partner={PartnerId}", partnerId);

            var active = await batchManager.GetActiveByPartnerAsync(partnerId, cancellationToken);
            if (active is null)
            {
                return NotFound();
            }

            return Ok(active.ToV2Dto());
        }

        /// <summary>
        /// Lists all generation batches for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("batches")]
        [ProducesResponseType(typeof(IEnumerable<AreaGenerationBatchDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBatches(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetBatches for Partner={PartnerId}", partnerId);

            var batches = await batchManager.GetByPartnerAsync(partnerId, cancellationToken);
            return Ok(batches.Select(b => b.ToV2Dto()));
        }

        /// <summary>
        /// Gets a specific batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="batchId">The batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("batches/{batchId}")]
        [ProducesResponseType(typeof(AreaGenerationBatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid partnerId, Guid batchId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetBatch Partner={PartnerId}, Batch={BatchId}", partnerId, batchId);

            var batch = await batchManager.GetAsync(batchId, cancellationToken);
            if (batch is null || batch.PartnerId != partnerId)
            {
                return NotFound();
            }

            return Ok(batch.ToV2Dto());
        }

        /// <summary>
        /// Cancels a running generation batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="batchId">The batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("batches/{batchId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CancelBatch(
            Guid partnerId,
            Guid batchId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CancelBatch Partner={PartnerId}, Batch={BatchId}", partnerId, batchId);

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

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
