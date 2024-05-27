﻿namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Route("api/partnerlocationservices")]
    public class PartnerLocationServicesController : SecureController
    {
        private readonly IKeyedManager<PartnerLocation> partnerLocationManager;
        private readonly IBaseManager<PartnerLocationService> partnerLocationServicesManager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerLocationServicesController(IBaseManager<PartnerLocationService> partnerLocationServicesManager,
            IKeyedManager<Partner> partnerManager,
            IKeyedManager<PartnerLocation> partnerLocationManager)
        {
            this.partnerLocationServicesManager = partnerLocationServicesManager;
            this.partnerManager = partnerManager;
            this.partnerLocationManager = partnerLocationManager;
        }

        [HttpGet("getbypartnerlocation/{partnerLocationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices =
                await partnerLocationServicesManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        [HttpGet("{partnerLocationId}/{serviceTypeId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken)
        {
            var partnerLocationService =
                await partnerLocationServicesManager.GetAsync(partnerLocationId, serviceTypeId, cancellationToken);

            return Ok(partnerLocationService);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PartnerLocationService partnerLocationService,
            CancellationToken cancellationToken)
        {
            var partnerLocation =
                await partnerLocationManager.GetAsync(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result =
                await partnerLocationServicesManager.AddAsync(partnerLocationService, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get),
                new
                {
                    partnerLocationId = partnerLocationService.PartnerLocationId,
                    serviceTypeId = partnerLocationService.ServiceTypeId,
                }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocationService(PartnerLocationService partnerLocationService,
            CancellationToken cancellationToken)
        {
            var partnerLocation =
                await partnerLocationManager.GetAsync(partnerLocationService.PartnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationServicesManager.UpdateAsync(partnerLocationService, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocationService));

            return Ok(partnerLocationService);
        }

        [HttpDelete("{partnerLocationId}/{serviceTypeId}")]
        public async Task<IActionResult> DeletePartnerLocationService(Guid partnerLocationId, int serviceTypeId,
            CancellationToken cancellationToken)
        {
            var partnerLocation = await partnerLocationManager.GetAsync(partnerLocationId, cancellationToken);
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationServicesManager.Delete(partnerLocationId, serviceTypeId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocationService));

            return Ok(partnerLocationId);
        }
    }
}