namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerservices")]
    public class PartnerServicesController : SecureController
    {
        private readonly IBaseManager<PartnerService> PartnerServicesManager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerServicesController(IBaseManager<PartnerService> partnerServicesManager,
                                         IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.PartnerServicesManager = partnerServicesManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("{partnerId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerServices(Guid partnerId, CancellationToken cancellationToken)
        {
            var PartnerServices = await PartnerServicesManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(PartnerServices);
        }

        [HttpGet("{partnerId}/{serviceTypeId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerService(Guid partnerId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var PartnerService = await PartnerServicesManager.GetAsync(partnerId, serviceTypeId, cancellationToken);

            return Ok(PartnerService);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerService(PartnerService partnerService, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerService.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await PartnerServicesManager.AddAsync(partnerService, cancellationToken);

            return CreatedAtAction(nameof(GetPartnerService), new { partnerId = partnerService.PartnerId, serviceTypeId = partnerService.ServiceTypeId, cancellationToken = cancellationToken }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerService(PartnerService partnerService, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerService.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await PartnerServicesManager.UpdateAsync(partnerService, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerService));

            return Ok(partnerService);
        }

        [HttpDelete("{partnerId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerService(Guid partnerId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await PartnerServicesManager.Delete(partnerId, serviceTypeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerService));

            return Ok(partnerId);
        }
    }
}
