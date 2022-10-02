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

        public PartnerServicesController(IBaseManager<PartnerService> PartnerServicesManager,
                                         IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.PartnerServicesManager = PartnerServicesManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("{PartnerId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerServices(Guid PartnerId, CancellationToken cancellationToken)
        {
            var PartnerServices = await PartnerServicesManager.GetByParentId(PartnerId, cancellationToken);

            return Ok(PartnerServices);
        }

        [HttpGet("{PartnerId}/{serviceTypeId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerService(Guid PartnerId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var PartnerService = await PartnerServicesManager.Get(PartnerId, serviceTypeId, cancellationToken);

            return Ok(PartnerService);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerService(PartnerService partnerService, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerService.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await PartnerServicesManager.Add(partnerService);

            return CreatedAtAction(nameof(GetPartnerService), new { PartnerId = partnerService.PartnerId, serviceTypeId = partnerService.ServiceTypeId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerService(PartnerService partnerService, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerService.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await PartnerServicesManager.Update(partnerService).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerService));

            return Ok(partnerService);
        }

        [HttpDelete("{PartnerId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerService(Guid partnerId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
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
