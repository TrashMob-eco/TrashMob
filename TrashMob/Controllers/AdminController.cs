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
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/admin")]
    public class AdminController : SecureController
    {
        private readonly IBaseManager<PartnerRequest> partnerRequestManager;

        public AdminController(IBaseManager<PartnerRequest> partnerRequestManager) : base()
        {
            this.partnerRequestManager = partnerRequestManager;
        }

        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize(Policy = "UserIsAdmin")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest, CancellationToken cancellationToken)
        {
            var result = await partnerRequestManager.UpdateAsync(partnerRequest, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerRequest));

            return Ok(result);
        }
    }
}
