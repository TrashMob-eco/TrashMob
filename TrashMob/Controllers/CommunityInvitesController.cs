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
    /// Controller for community email invitation operations.
    /// Endpoints require community admin privileges.
    /// </summary>
    [Authorize]
    [Route("api/communities/{communityId}/invites")]
    public class CommunityInvitesController : SecureController
    {
        private readonly IEmailInviteManager emailInviteManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityInvitesController"/> class.
        /// </summary>
        /// <param name="emailInviteManager">The email invite manager.</param>
        /// <param name="partnerManager">The partner manager for authorization.</param>
        public CommunityInvitesController(
            IEmailInviteManager emailInviteManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.emailInviteManager = emailInviteManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all email invite batches for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches for the community with summary statistics.</returns>
        [HttpGet("batches")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailInviteBatch>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBatches(Guid communityId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await emailInviteManager.GetCommunityBatchesAsync(communityId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(GetBatches));

            return Ok(result);
        }

        /// <summary>
        /// Gets a batch with its individual invite details for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch with invites or 404 if not found.</returns>
        [HttpGet("batches/{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid communityId, Guid id, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken).ConfigureAwait(false);

            if (result == null || result.CommunityId != communityId)
            {
                return NotFound();
            }

            TrackEvent(nameof(GetBatch));

            return Ok(result);
        }

        /// <summary>
        /// Creates and sends a new batch of email invites for a community.
        /// </summary>
        /// <param name="communityId">The community (partner) ID.</param>
        /// <param name="request">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        [HttpPost("batch")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateBatch(
            Guid communityId,
            [FromBody] CreateEmailInviteBatchRequest request,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(communityId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            if (request == null || request.Emails == null || !request.Emails.Any())
            {
                return BadRequest("Email list is required.");
            }

            // Create the batch with community context
            var batch = await emailInviteManager.CreateBatchAsync(
                request.Emails,
                UserId,
                "Community",
                communityId,
                null,
                cancellationToken).ConfigureAwait(false);

            // Process the batch (send emails)
            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken).ConfigureAwait(false);

            TrackEvent(nameof(CreateBatch));

            return CreatedAtAction(nameof(GetBatch), new { communityId, id = result.Id }, result);
        }
    }
}
