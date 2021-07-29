namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Authorize]
    [Route("api/partnerusers")]
    public class PartnerUsersController : ControllerBase
    {
        private readonly IPartnerUserRepository partnerUserRepository;
        private readonly IUserRepository userRepository;

        public PartnerUsersController(IPartnerUserRepository partnerUserRepository, IUserRepository userRepository)
        {
            this.partnerUserRepository = partnerUserRepository;
            this.userRepository = userRepository;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerUsers(Guid partnerId)
        {
            return Ok(await partnerUserRepository.GetPartnerUsers(partnerId).ConfigureAwait(false));
        }

        [HttpGet("{partnerId}/{userId}")]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId)
        {
            var partnerUser = await partnerUserRepository.GetPartnerUser(partnerId, userId).ConfigureAwait(false);

            if (partnerUser == null)
            {
                return NotFound();
            }

            return Ok(partnerUser);
        }

        [HttpPost("{partnerId}/{userId}")]

        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var currentUser = await userRepository.GetUserByNameIdentifier(User.FindFirst(ClaimTypes.NameIdentifier).Value).ConfigureAwait(false);

            if (!currentUser.IsSiteAdmin)
            {
                var currentUserPartner = await partnerUserRepository.GetPartnerUser(partnerId, currentUser.Id).ConfigureAwait(false);

                if (currentUserPartner == null)
                {
                    return Forbid();
                }
            }

            var partnerUser = new PartnerUser()
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = currentUser.Id,
                LastUpdatedByUserId = currentUser.Id
            };

            await partnerUserRepository.AddPartnerUser(partnerUser).ConfigureAwait(false);

            return Ok();
        }
    }
}
