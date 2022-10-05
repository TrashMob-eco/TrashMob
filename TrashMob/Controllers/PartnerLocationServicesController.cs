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
 
    [Route("api/partnerlocationservices")]
    public class PartnerLocationServicesController : SecureController
    {
        private readonly IBaseManager<PartnerLocationService> partnerLocationServicesManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;

        public PartnerLocationServicesController(IBaseManager<PartnerLocationService> partnerLocationServicesManager,
                                                 IKeyedManager<Partner> partnerManager,
                                                 IKeyedManager<PartnerLocation> partnerLocationManager)
            : base()
        {
            this.partnerLocationServicesManager = partnerLocationServicesManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("{partnerLocationId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> Get(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices = await partnerLocationServicesManager.GetByParentId(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        [HttpGet("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> Get(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var partnerLocationService = await partnerLocationServicesManager.Get(partnerLocationId, serviceTypeId, cancellationToken);

            return Ok(partnerLocationService);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PartnerLocationService partnerLocationService, CancellationToken cancellationToken)
        {
            var partnerLocation = await partnerLocationManager.Get(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.Get(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationServicesManager.Add(partnerLocationService);

            return CreatedAtAction(nameof(Get), new { partnerLocationId = partnerLocationService.PartnerLocationId, serviceTypeId = partnerLocationService.ServiceTypeId });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocationService(PartnerLocationService partnerLocationService, CancellationToken cancellationToken)
        {
            var partnerLocation = await partnerLocationManager.Get(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.Get(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationServicesManager.Update(partnerLocationService).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocationService));

            return Ok(partnerLocationService);
        }

        [HttpDelete("{partnerLocationId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerLocationService(Guid partnerLocationId, int serviceTypeId, CancellationToken cancellationToken)
        {
            var partnerLocation = await partnerLocationManager.Get(partnerLocationId, cancellationToken);
            var partner = await partnerManager.Get(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationServicesManager.Delete(partnerLocationId, serviceTypeId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocationService));

            return Ok(partnerLocationId);
        }
    }
}
