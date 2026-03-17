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
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for reviewing staged adoptable areas produced by AI generation.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{partnerId}/areas/staged")]
    public class StagedAreasV2Controller(
        IStagedAdoptableAreaManager stagedManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<StagedAreasV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets staged areas for a batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="batchId">The batch ID to filter by.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StagedAdoptableAreaDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStagedAreas(
            Guid partnerId,
            [FromQuery] Guid batchId,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetStagedAreas Partner={PartnerId}, Batch={BatchId}", partnerId, batchId);

            var areas = await stagedManager.GetByBatchAsync(batchId, cancellationToken);
            return Ok(areas.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Approves a single staged area.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The staged area ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(
            Guid partnerId,
            Guid id,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Approve staged area Partner={PartnerId}, Id={Id}", partnerId, id);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await stagedManager.ApproveAsync(id, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Rejects a single staged area.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The staged area ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(
            Guid partnerId,
            Guid id,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Reject staged area Partner={PartnerId}, Id={Id}", partnerId, id);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await stagedManager.RejectAsync(id, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Bulk-approves staged areas in a batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="request">The bulk review request with batch ID and optional area IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("approve-batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkApprove(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 BulkApprove Partner={PartnerId}, Batch={BatchId}", partnerId, request.BatchId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var count = await stagedManager.BulkApproveAsync(request.BatchId, UserId, request.Ids, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Bulk-rejects staged areas in a batch.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="request">The bulk review request with batch ID and optional area IDs.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("reject-batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkReject(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 BulkReject Partner={PartnerId}, Batch={BatchId}", partnerId, request.BatchId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var count = await stagedManager.BulkRejectAsync(request.BatchId, UserId, request.Ids, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Updates the name of a staged area.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="id">The staged area ID.</param>
        /// <param name="request">The request containing the new name.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}/name")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateName(
            Guid partnerId,
            Guid id,
            [FromBody] UpdateStagedAreaNameRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateName staged area Partner={PartnerId}, Id={Id}", partnerId, id);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await stagedManager.UpdateNameAsync(id, request.Name, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Promotes all approved staged areas to real adoptable areas.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="request">The bulk review request with batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("create-approved")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaBulkImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateApprovedAreas(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CreateApprovedAreas Partner={PartnerId}, Batch={BatchId}", partnerId, request.BatchId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await stagedManager.CreateApprovedAreasAsync(request.BatchId, UserId, cancellationToken);
            return Ok(result);
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
