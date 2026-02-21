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
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Route("api/admin")]
    public class AdminController(
        IKeyedManager<PartnerRequest> partnerRequestManager,
        IEmailManager emailManager)
        : SecureController
    {

        /// <summary>
        /// Updates a partner request. Admin only.
        /// </summary>
        /// <param name="userId">The ID of the user performing the update.</param>
        /// <param name="partnerRequest">The partner request to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the updated partner request.</remarks>
        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest,
            CancellationToken cancellationToken)
        {
            var result = await partnerRequestManager.UpdateAsync(partnerRequest, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerRequest));

            return Ok(result);
        }

        /// <summary>
        /// Gets a list of all email templates. Admin only.
        /// </summary>
        [HttpGet("emailTemplates")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(IEnumerable<EmailTemplate>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEmails(CancellationToken cancellationToken)
        {
            var result = await emailManager.GetEmailTemplatesAsync(cancellationToken);
            TrackEvent(nameof(GetEmails));

            return Ok(result);
        }
    }
}