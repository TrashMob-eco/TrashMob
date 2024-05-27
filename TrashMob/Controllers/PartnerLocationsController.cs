﻿namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerlocations")]
    public class PartnerLocationsController : SecureController
    {
        private readonly IPartnerLocationManager partnerLocationManager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerLocationsController(IPartnerLocationManager partnerLocationManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.partnerLocationManager = partnerLocationManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerLocations(Guid partnerId, CancellationToken cancellationToken)
        {
            var results = await partnerLocationManager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{partnerLocationId}")]
        public async Task<IActionResult> GetPartnerLocation(Guid partnerLocationId,
            CancellationToken cancellationToken = default)
        {
            var partnerLocation =
                (await partnerLocationManager.GetAsync(pl => pl.Id == partnerLocationId, cancellationToken))
                .FirstOrDefault();

            if (partnerLocation == null)
            {
                return NotFound();
            }

            return Ok(partnerLocation);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocation(PartnerLocation partnerLocation,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerLocationManager.AddAsync(partnerLocation, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerLocation));

            return CreatedAtAction(nameof(GetPartnerLocation), new { partnerLocationId = partnerLocation.Id }, result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocation(PartnerLocation partnerLocation,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner =
                await partnerLocationManager.GetPartnerForLocationAsync(partnerLocation.Id, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationManager.UpdateAsync(partnerLocation, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocation));

            return Ok(partnerLocation);
        }

        [HttpDelete("{partnerLocationId}")]
        public async Task<IActionResult> DeletePartnerLocation(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationManager.DeleteAsync(partnerLocationId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocation));

            return Ok(partnerLocationId);
        }
    }
}