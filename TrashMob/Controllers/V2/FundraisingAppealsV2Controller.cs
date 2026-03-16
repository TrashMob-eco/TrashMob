namespace TrashMob.Controllers.V2
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// V2 admin controller for sending fundraising appeal emails.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/fundraising-appeals")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class FundraisingAppealsV2Controller(
        IDonationEmailManager donationEmailManager,
        ILogger<FundraisingAppealsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Sends a fundraising appeal email to a single contact.
        /// </summary>
        /// <param name="request">The appeal request containing contact, subject, and body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Appeal sent successfully.</response>
        [HttpPost("send")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Send([FromBody] AppealRequestDto request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Send Appeal to Contact={ContactId} by User={UserId}", request.ContactId, UserId);

            await donationEmailManager.SendAppealAsync(request.ContactId, request.Subject, request.Body, UserId, cancellationToken);

            return NoContent();
        }

        /// <summary>
        /// Sends a fundraising appeal email to multiple contacts.
        /// </summary>
        /// <param name="request">The bulk appeal request containing contact IDs, subject, and body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A summary of the bulk send operation.</returns>
        /// <response code="200">Returns bulk send results.</response>
        [HttpPost("send-bulk")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(BulkAppealResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendBulk([FromBody] BulkAppealRequestDto request, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 SendBulk Appeal to {Count} contacts by User={UserId}", request.ContactIds.Count, UserId);

            var result = await donationEmailManager.SendBulkAppealAsync(request.ContactIds, request.Subject, request.Body, UserId, cancellationToken);

            return Ok(result);
        }
    }
}
