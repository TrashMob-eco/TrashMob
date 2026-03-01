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
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// Controller for sending fundraising appeal emails to contacts.
    /// </summary>
    [Route("api/fundraising-appeals")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class FundraisingAppealsController(IDonationEmailManager donationEmailManager)
        : SecureController
    {
        /// <summary>
        /// Sends a fundraising appeal email to a single contact.
        /// </summary>
        /// <param name="request">The appeal request with contact ID, subject, and body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("send")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Send(AppealRequest request, CancellationToken cancellationToken)
        {
            await donationEmailManager.SendAppealAsync(request.ContactId, request.Subject, request.Body, UserId, cancellationToken);
            TrackEvent("SendFundraisingAppeal");
            return NoContent();
        }

        /// <summary>
        /// Sends a fundraising appeal email to multiple contacts.
        /// </summary>
        /// <param name="request">The bulk appeal request with contact IDs, subject, and body.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("send-bulk")]
        [ProducesResponseType(typeof(BulkAppealResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> SendBulk(BulkAppealRequest request, CancellationToken cancellationToken)
        {
            var result = await donationEmailManager.SendBulkAppealAsync(request.ContactIds, request.Subject, request.Body, UserId, cancellationToken);
            TrackEvent("SendBulkFundraisingAppeal");
            return Ok(result);
        }
    }

    /// <summary>
    /// Request model for sending a fundraising appeal to a single contact.
    /// </summary>
    public class AppealRequest
    {
        /// <summary>
        /// Gets or sets the contact ID to send the appeal to.
        /// </summary>
        public Guid ContactId { get; set; }

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        public string Body { get; set; } = string.Empty;
    }

    /// <summary>
    /// Request model for sending a fundraising appeal to multiple contacts.
    /// </summary>
    public class BulkAppealRequest
    {
        /// <summary>
        /// Gets or sets the list of contact IDs to send the appeal to.
        /// </summary>
        public List<Guid> ContactIds { get; set; } = [];

        /// <summary>
        /// Gets or sets the email subject line.
        /// </summary>
        public string Subject { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the email body content.
        /// </summary>
        public string Body { get; set; } = string.Empty;
    }
}
