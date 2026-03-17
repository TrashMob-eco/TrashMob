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
    /// V2 controller for community email invitation operations.
    /// Endpoints require community admin privileges.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/communities/{communityId}/invites")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class CommunityInvitesV2Controller(
        IEmailInviteManager emailInviteManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<CommunityInvitesV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all email invite batches for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches for the community with summary statistics.</returns>
        /// <response code="200">Returns the batch list.</response>
        /// <response code="403">Not authorized for this community.</response>
        [HttpGet("batches")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<EmailInviteBatchDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBatches(Guid communityId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            logger.LogInformation("V2 GetBatches for community {CommunityId}", communityId);

            var batches = await emailInviteManager.GetCommunityBatchesAsync(communityId, cancellationToken);
            var dtos = batches.Select(b => b.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a batch with its individual invite details for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch or 404 if not found.</returns>
        /// <response code="200">Returns the batch.</response>
        /// <response code="403">Not authorized for this community.</response>
        /// <response code="404">Batch not found.</response>
        [HttpGet("batches/{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid communityId, Guid id, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var batch = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken);

            if (batch is null || batch.CommunityId != communityId)
            {
                return NotFound();
            }

            logger.LogInformation("V2 GetBatch {BatchId} for community {CommunityId}", id, communityId);

            return Ok(batch.ToV2Dto());
        }

        /// <summary>
        /// Creates and sends a new batch of email invites for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="dto">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        /// <response code="201">Batch created and sent.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="403">Not authorized for this community.</response>
        [HttpPost("batch")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBatch(
            Guid communityId,
            [FromBody] CreateEmailInviteBatchDto dto,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (dto is null || dto.Emails is null || !dto.Emails.Any())
            {
                return Problem("Email list is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            logger.LogInformation("V2 CreateBatch: {Count} emails for community {CommunityId}",
                dto.Emails.Count(), communityId);

            var batch = await emailInviteManager.CreateBatchAsync(
                dto.Emails,
                UserId,
                "Community",
                communityId,
                null,
                cancellationToken);

            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken);

            return CreatedAtAction(nameof(GetBatch), new { communityId, id = result.Id }, result.ToV2Dto());
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
