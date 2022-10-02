namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared;
    using TrashMob.Shared.Persistence.Interfaces;

    [Route("api/admin")]
    public class AdminController : SecureController
    {
        private readonly IPartnerRequestRepository partnerRequestRepository;

        public AdminController(TelemetryClient telemetryClient,
                               IAuthorizationService authorizationService,
                               IPartnerRequestRepository partnerRequestRepository)
            : base(telemetryClient, authorizationService)
        {
            this.partnerRequestRepository = partnerRequestRepository;
        }

        [HttpPut("partnerrequestupdate/{userId}")]
        [Authorize(Policy = "UserIsAdmin")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        public async Task<IActionResult> UpdatePartnerRequest(Guid userId, PartnerRequest partnerRequest)
        {
            var result = await partnerRequestRepository.UpdatePartnerRequest(partnerRequest).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerRequest));

            return Ok(result);
        }
    }
}
