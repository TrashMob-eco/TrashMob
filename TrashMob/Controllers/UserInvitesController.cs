namespace TrashMob.Controllers
{
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
    /// Controller for user-initiated email invitations.
    /// Allows any authenticated user to invite friends with rate limits.
    /// </summary>
    [Route("api/invites")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class UserInvitesController(IEmailInviteManager emailInviteManager)
        : SecureController
    {
        /// <summary>
        /// Maximum number of emails per batch for user invites.
        /// </summary>
        private const int MaxEmailsPerBatch = 10;

        /// <summary>
        /// Maximum number of invites per month for each user.
        /// </summary>
        private const int MaxInvitesPerMonth = 50;

        /// <summary>
        /// Gets the current user's invite batches.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches sent by the user.</returns>
        [HttpGet("batches")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailInviteBatch>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBatches(CancellationToken cancellationToken = default)
        {
            var result = await emailInviteManager.GetUserBatchesAsync(UserId, cancellationToken);
            TrackEvent(nameof(GetBatches));

            return Ok(result);
        }

        /// <summary>
        /// Gets a batch with its individual invite details.
        /// User can only view their own batches.
        /// </summary>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch with invites or 404 if not found.</returns>
        [HttpGet("batches/{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetBatch(System.Guid id, CancellationToken cancellationToken = default)
        {
            var result = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            // Users can only view their own batches
            if (result.SenderUserId != UserId)
            {
                return Forbid();
            }

            TrackEvent(nameof(GetBatch));

            return Ok(result);
        }

        /// <summary>
        /// Gets the user's remaining invite quota for the month.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quota information.</returns>
        [HttpGet("quota")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(UserInviteQuota), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuota(CancellationToken cancellationToken = default)
        {
            var monthlyCount = await emailInviteManager.GetUserMonthlyInviteCountAsync(UserId, cancellationToken);
            var remaining = MaxInvitesPerMonth - monthlyCount;

            var quota = new UserInviteQuota
            {
                MaxPerBatch = MaxEmailsPerBatch,
                MaxPerMonth = MaxInvitesPerMonth,
                UsedThisMonth = monthlyCount,
                RemainingThisMonth = remaining > 0 ? remaining : 0,
            };

            TrackEvent(nameof(GetQuota));

            return Ok(quota);
        }

        /// <summary>
        /// Creates and sends a new batch of email invites.
        /// Limited to 10 emails per batch and 50 per month.
        /// </summary>
        /// <param name="request">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch with send results.</returns>
        [HttpPost("batch")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatch), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> CreateBatch(
            [FromBody] CreateEmailInviteBatchRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request is null || request.Emails is null || !request.Emails.Any())
            {
                return BadRequest("Email list is required.");
            }

            var emailCount = request.Emails.Count();

            // Check batch size limit
            if (emailCount > MaxEmailsPerBatch)
            {
                return BadRequest($"Maximum {MaxEmailsPerBatch} emails per batch allowed.");
            }

            // Check monthly quota
            var monthlyCount = await emailInviteManager.GetUserMonthlyInviteCountAsync(UserId, cancellationToken);
            var remaining = MaxInvitesPerMonth - monthlyCount;

            if (remaining <= 0)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = "Monthly invite limit reached. Please try again next month.",
                    limit = MaxInvitesPerMonth,
                    used = monthlyCount,
                });
            }

            if (emailCount > remaining)
            {
                return StatusCode(StatusCodes.Status429TooManyRequests, new
                {
                    message = $"You can only send {remaining} more invite(s) this month.",
                    limit = MaxInvitesPerMonth,
                    used = monthlyCount,
                    remaining,
                    requested = emailCount,
                });
            }

            // Create the batch
            var batch = await emailInviteManager.CreateBatchAsync(
                request.Emails,
                UserId,
                "User",
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
    /// Response model for user invite quota.
    /// </summary>
    public class UserInviteQuota
    {
        /// <summary>
        /// Gets or sets the maximum emails allowed per batch.
        /// </summary>
        public int MaxPerBatch { get; set; }

        /// <summary>
        /// Gets or sets the maximum invites allowed per month.
        /// </summary>
        public int MaxPerMonth { get; set; }

        /// <summary>
        /// Gets or sets the number of invites used this month.
        /// </summary>
        public int UsedThisMonth { get; set; }

        /// <summary>
        /// Gets or sets the remaining invites this month.
        /// </summary>
        public int RemainingThisMonth { get; set; }
    }
}
