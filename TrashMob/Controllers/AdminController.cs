namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/admin")]
    public class AdminController : SecureController
    {
        private readonly IKeyedManager<PartnerRequest> partnerRequestManager;
        private readonly IEmailManager emailManager;

        public AdminController(IKeyedManager<PartnerRequest> partnerRequestManager, IEmailManager emailManager) : base()
        {
            this.partnerRequestManager = partnerRequestManager;
            this.emailManager = emailManager;
        }

        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest, CancellationToken cancellationToken)
        {
            var result = await partnerRequestManager.UpdateAsync(partnerRequest, UserId, cancellationToken).ConfigureAwait(false);
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
