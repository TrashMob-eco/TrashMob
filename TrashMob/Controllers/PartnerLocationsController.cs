namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Authorize]
    [Route("api/partnerlocations")]
    public class PartnerLocationsController : ControllerBase
    {
        private readonly IPartnerLocationRepository partnerLocationRepository;
        private readonly IUserRepository userRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public PartnerLocationsController(IPartnerLocationRepository partnerLocationRepository, IPartnerUserRepository partnerUserRepository, IUserRepository userRepository)
        {
            this.partnerLocationRepository = partnerLocationRepository;
            this.partnerUserRepository = partnerUserRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{partnerId}")]
        public IActionResult GetPartnerLocations(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(partnerLocationRepository.GetPartnerLocations(cancellationToken).Where(pl => pl.PartnerId == partnerId).ToList());
        }

        [HttpGet("{partnerId}/{locationId}")]
        public IActionResult GetPartnerLocation(Guid partnerId, Guid locationId, CancellationToken cancellationToken)
        {
            var partnerLocation = partnerLocationRepository.GetPartnerLocations(cancellationToken).FirstOrDefault(pl => pl.PartnerId == partnerId && pl.Id == locationId);

            if (partnerLocation == null)
            {
                return NotFound();
            }

            return Ok(partnerLocation);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocation(PartnerLocation partnerLocation)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == partnerLocation.PartnerId && pu.UserId == currentUser.Id);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            await partnerLocationRepository.AddPartnerLocation(partnerLocation).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetPartnerLocation), new { partnerId = partnerLocation.PartnerId, locationId = partnerLocation.Id });
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocation(PartnerLocation partnerLocation)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == partnerLocation.PartnerId && pu.UserId == currentUser.Id);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            await partnerLocationRepository.UpdatePartnerLocation(partnerLocation).ConfigureAwait(false);

            return Ok(partnerLocation);
        }

        [HttpDelete("{partnerId}/{partnerLocationId}")]
        public async Task<IActionResult> DeletePartnerLocation(Guid partnerId, Guid partnerLocationId)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.PartnerId == partnerId && pu.UserId == currentUser.Id);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            await partnerLocationRepository.DeletePartnerLocation(partnerLocationId).ConfigureAwait(false);

            return Ok(partnerLocationId);
        }
    }
}
