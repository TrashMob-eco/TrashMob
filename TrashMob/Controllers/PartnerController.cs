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
    [Route("api/partners")]
    public class PartnersController : ControllerBase
    {
        private readonly IPartnerRepository partnerRepository;
        private readonly IUserRepository userRepository;
        private readonly IPartnerUserRepository partnerUserRepository;

        public PartnersController(IPartnerRepository partnerRepository, IUserRepository userRepository, IPartnerUserRepository partnerUserRepository)
        {
            this.partnerRepository = partnerRepository;
            this.userRepository = userRepository;
            this.partnerUserRepository = partnerUserRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPartners(CancellationToken cancellationToken)
        {
            return Ok(await partnerRepository.GetPartners(cancellationToken).ConfigureAwait(false));
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(await partnerRepository.GetPartner(partnerId, cancellationToken).ConfigureAwait(false));
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdatePartner(Partner partner)
        {
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (currentUser == null)
            {
                return Forbid();
            }

            // Ensure user is allowed to update this Partner
            if (!currentUser.IsSiteAdmin)
            {
                var partnerUser = partnerUserRepository.GetPartnerUsers().FirstOrDefault(pu => pu.UserId == currentUser.Id && pu.PartnerId == partner.Id);

                if (partnerUser == null)
                {
                    return Forbid();
                }
            }

            return Ok(await partnerRepository.UpdatePartner(partner).ConfigureAwait(false));
        }
    }
}
