namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerlocations")]
    public class PartnerLocationsController : SecureController
    {
        private readonly IKeyedManager<PartnerLocation> partnerLocationRepository;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerLocationsController(IKeyedManager<PartnerLocation> partnerLocationManager,
                                          IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.partnerLocationRepository = partnerLocationManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerLocations(Guid partnerId, CancellationToken cancellationToken)
        {
            var results = await partnerLocationRepository.Get(pl => pl.PartnerId == partnerId, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{partnerId}/{locationId}")]
        public async Task<IActionResult> GetPartnerLocation(Guid partnerId, Guid locationId, CancellationToken cancellationToken = default)
        {
            var partnerLocation = (await partnerLocationRepository.Get(pl => pl.PartnerId == partnerId && pl.Id == locationId, cancellationToken)).FirstOrDefault();

            if (partnerLocation == null)
            {
                return NotFound();
            }

            return Ok(partnerLocation);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocation(PartnerLocation partnerLocation, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationRepository.Add(partnerLocation).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerLocation));

            return CreatedAtAction(nameof(GetPartnerLocation), new { partnerId = partnerLocation.PartnerId, locationId = partnerLocation.Id });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocation(PartnerLocation partnerLocation, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.Get(partnerLocation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationRepository.Update(partnerLocation).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocation));

            return Ok(partnerLocation);
        }

        [HttpDelete("{partnerId}/{partnerLocationId}")]
        public async Task<IActionResult> DeletePartnerLocation(Guid partnerId, Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationRepository.Delete(partnerLocationId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocation));

            return Ok(partnerLocationId);
        }
    }
}
