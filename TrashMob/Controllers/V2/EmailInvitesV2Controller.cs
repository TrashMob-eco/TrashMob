namespace TrashMob.Controllers.V2
{
    using System;
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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for user-initiated email invitations with rate limiting.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/invites")]
    [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
    public class EmailInvitesV2Controller(
        IEmailInviteManager emailInviteManager,
        ILogger<EmailInvitesV2Controller> logger) : ControllerBase
    {
        private const int MaxEmailsPerBatch = 10;
        private const int MaxInvitesPerMonth = 50;

        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets the current user's invite batches.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of batches sent by the user.</returns>
        /// <response code="200">Returns the batch list.</response>
        [HttpGet("batches")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(System.Collections.Generic.IReadOnlyList<EmailInviteBatchDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetBatches(CancellationToken cancellationToken = default)
        {
            logger.LogInformation("V2 GetBatches for user {UserId}", UserId);

            var batches = await emailInviteManager.GetUserBatchesAsync(UserId, cancellationToken);
            var dtos = batches.Select(b => b.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Gets a batch with its details. Users can only view their own batches.
        /// </summary>
        /// <param name="id">Batch ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The batch or 404.</returns>
        /// <response code="200">Returns the batch.</response>
        /// <response code="403">User does not own this batch.</response>
        /// <response code="404">Batch not found.</response>
        [HttpGet("batches/{id}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBatch(Guid id, CancellationToken cancellationToken = default)
        {
            var batch = await emailInviteManager.GetBatchDetailsAsync(id, cancellationToken);

            if (batch is null)
            {
                return NotFound();
            }

            if (batch.SenderUserId != UserId)
            {
                return Forbid();
            }

            return Ok(batch.ToV2Dto());
        }

        /// <summary>
        /// Gets the user's remaining invite quota for the month.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Quota information.</returns>
        /// <response code="200">Returns quota info.</response>
        [HttpGet("quota")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(EmailInviteQuotaDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetQuota(CancellationToken cancellationToken = default)
        {
            var monthlyCount = await emailInviteManager.GetUserMonthlyInviteCountAsync(UserId, cancellationToken);
            var remaining = MaxInvitesPerMonth - monthlyCount;

            var quota = new EmailInviteQuotaDto
            {
                MaxPerBatch = MaxEmailsPerBatch,
                MaxPerMonth = MaxInvitesPerMonth,
                UsedThisMonth = monthlyCount,
                RemainingThisMonth = remaining > 0 ? remaining : 0,
            };

            return Ok(quota);
        }

        /// <summary>
        /// Creates and sends a new batch of email invites.
        /// Limited to 10 emails per batch and 50 per month.
        /// </summary>
        /// <param name="dto">The batch creation request.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created batch.</returns>
        /// <response code="201">Batch created and sent.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="429">Rate limit exceeded.</response>
        [HttpPost("batch")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(EmailInviteBatchDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> CreateBatch([FromBody] CreateEmailInviteBatchDto dto, CancellationToken cancellationToken = default)
        {
            if (dto is null || dto.Emails is null || !dto.Emails.Any())
            {
                return Problem("Email list is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            var emailCount = dto.Emails.Count();

            if (emailCount > MaxEmailsPerBatch)
            {
                return Problem($"Maximum {MaxEmailsPerBatch} emails per batch allowed.", statusCode: StatusCodes.Status400BadRequest);
            }

            var monthlyCount = await emailInviteManager.GetUserMonthlyInviteCountAsync(UserId, cancellationToken);
            var remaining = MaxInvitesPerMonth - monthlyCount;

            if (remaining <= 0)
            {
                return Problem("Monthly invite limit reached. Please try again next month.", statusCode: StatusCodes.Status429TooManyRequests);
            }

            if (emailCount > remaining)
            {
                return Problem($"You can only send {remaining} more invite(s) this month.", statusCode: StatusCodes.Status429TooManyRequests);
            }

            logger.LogInformation("V2 CreateBatch: {Count} emails for user {UserId}", emailCount, UserId);

            var batch = await emailInviteManager.CreateBatchAsync(
                dto.Emails,
                UserId,
                "User",
                null,
                null,
                cancellationToken);

            var result = await emailInviteManager.ProcessBatchAsync(batch.Id, cancellationToken);

            return StatusCode(StatusCodes.Status201Created, result.ToV2Dto());
        }
    }
}
