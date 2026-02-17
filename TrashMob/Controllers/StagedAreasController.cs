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
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for reviewing staged adoptable areas produced by AI generation.
    /// </summary>
    [Route("api/communities/{partnerId}/areas/staged")]
    public class StagedAreasController(
        IStagedAdoptableAreaManager stagedManager,
        IKeyedManager<Partner> partnerManager)
        : SecureController
    {
        /// <summary>
        /// Gets staged areas for a batch.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<StagedAdoptableArea>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetStagedAreas(
            Guid partnerId,
            [FromQuery] Guid batchId,
            CancellationToken cancellationToken)
        {
            var areas = await stagedManager.GetByBatchAsync(batchId, cancellationToken);
            return Ok(areas);
        }

        /// <summary>
        /// Approves a single staged area.
        /// </summary>
        [HttpPut("{id}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Approve(
            Guid partnerId,
            Guid id,
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

            await stagedManager.ApproveAsync(id, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Rejects a single staged area.
        /// </summary>
        [HttpPut("{id}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Reject(
            Guid partnerId,
            Guid id,
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

            await stagedManager.RejectAsync(id, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Bulk-approves staged areas in a batch.
        /// </summary>
        [HttpPost("approve-batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkApprove(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
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

            var count = await stagedManager.BulkApproveAsync(request.BatchId, UserId, request.Ids, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Bulk-rejects staged areas in a batch.
        /// </summary>
        [HttpPost("reject-batch")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(int), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> BulkReject(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
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

            var count = await stagedManager.BulkRejectAsync(request.BatchId, UserId, request.Ids, cancellationToken);
            return Ok(count);
        }

        /// <summary>
        /// Updates the name of a staged area.
        /// </summary>
        [HttpPut("{id}/name")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateName(
            Guid partnerId,
            Guid id,
            [FromBody] UpdateStagedAreaNameRequest request,
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

            await stagedManager.UpdateNameAsync(id, request.Name, UserId, cancellationToken);
            return NoContent();
        }

        /// <summary>
        /// Promotes all approved staged areas to real adoptable areas.
        /// </summary>
        [HttpPost("create-approved")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(AreaBulkImportResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateApprovedAreas(
            Guid partnerId,
            [FromBody] BulkReviewRequest request,
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

            var result = await stagedManager.CreateApprovedAreasAsync(request.BatchId, UserId, cancellationToken);
            return Ok(result);
        }
    }
}
