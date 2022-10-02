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
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerlocations")]
    public class PartnerLocationsController : SecureController
    {
        private readonly IPartnerLocationRepository partnerLocationRepository;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerLocationsController(IPartnerLocationRepository partnerLocationRepository,
                                          IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.partnerLocationRepository = partnerLocationRepository;
            this.partnerManager = partnerManager;
        }

        [HttpGet("{partnerId}")]
        public IActionResult GetPartnerLocations(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(partnerLocationRepository.GetPartnerLocations(cancellationToken).Where(pl => pl.PartnerId == partnerId).ToList());
        }

        [HttpGet("{partnerId}/{locationId}")]
        public IActionResult GetPartnerLocation(Guid partnerId, Guid locationId, CancellationToken cancellationToken = default)
        {
            var partnerLocation = partnerLocationRepository.GetPartnerLocations(cancellationToken).FirstOrDefault(pl => pl.PartnerId == partnerId && pl.Id == locationId);

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

            await partnerLocationRepository.AddPartnerLocation(partnerLocation).ConfigureAwait(false);
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

            await partnerLocationRepository.UpdatePartnerLocation(partnerLocation).ConfigureAwait(false);
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

            await partnerLocationRepository.DeletePartnerLocation(partnerLocationId).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocation));

            return Ok(partnerLocationId);
        }
    }
}
