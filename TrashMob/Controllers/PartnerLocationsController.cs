namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Linq;
    using System.Security.Claims;
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
        private readonly IPartnerRepository partnerRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public PartnerLocationsController(IPartnerLocationRepository partnerLocationRepository, IPartnerRepository partnerRepository, IPartnerUserRepository partnerUserRepository)
        {
            this.partnerLocationRepository = partnerLocationRepository;
            this.partnerRepository = partnerRepository;
            this.partnerUserRepository = partnerUserRepository;
        }

        [HttpGet("{partnerId}")]
        public IActionResult GetPartnerLocations(Guid partnerId)
        {
            return Ok(partnerLocationRepository.GetPartnerLocations().Where(pl => pl.PartnerId == partnerId).ToList());
        }

        [HttpGet("{partnerId}/{locationId}")]
        public IActionResult GetPartnerLocation(Guid partnerId, Guid locationId)
        {
            var partnerLocation = partnerLocationRepository.GetPartnerLocations().FirstOrDefault(pl => pl.PartnerId == partnerId && pl.Id == locationId);

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

            return Ok();
        }
    }
}
