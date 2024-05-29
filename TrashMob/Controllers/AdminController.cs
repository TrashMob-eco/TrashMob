namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/admin")]
    public class AdminController : SecureController
    {
        private readonly IEmailManager emailManager;
        private readonly IKeyedManager<PartnerRequest> partnerRequestManager;

        public AdminController(IKeyedManager<PartnerRequest> partnerRequestManager, IEmailManager emailManager)
        {
            this.partnerRequestManager = partnerRequestManager;
            this.emailManager = emailManager;
        }

        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest,
            CancellationToken cancellationToken)
        {
            var result = await partnerRequestManager.UpdateAsync(partnerRequest, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerRequest));

            return Ok(result);
        }

        [HttpGet("emailTemplates")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> GetEmails(CancellationToken cancellationToken)
        {
            var result = await emailManager.GetEmailTemplatesAsync(cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(GetEmails));

            return Ok(result);
        }
    }
}