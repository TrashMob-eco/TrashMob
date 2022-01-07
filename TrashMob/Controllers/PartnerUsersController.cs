namespace TrashMob.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Claims;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Poco;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

    [ApiController]
    [Authorize]
    [Route("api/partnerusers")]
    public class PartnerUsersController : ControllerBase
    {
        private readonly IPartnerUserRepository partnerUserRepository;
        private readonly IUserRepository userRepository;
        private readonly IPartnerRepository partnerRepository;

        public PartnerUsersController(IPartnerUserRepository partnerUserRepository, IUserRepository userRepository, IPartnerRepository partnerRepository)
        {
            this.partnerUserRepository = partnerUserRepository;
            this.userRepository = userRepository;
            this.partnerRepository = partnerRepository;
        }

        [HttpGet("{partnerId}")]
        public IActionResult GetPartnerUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(partnerUserRepository.GetPartnerUsers(cancellationToken).Where(pu => pu.PartnerId == partnerId).ToList());
        }

        [HttpGet("getpartnersforuser/{userId}")]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            var partnerUsers = partnerUserRepository.GetPartnerUsers(cancellationToken).Where(pu => pu.UserId == userId).ToList();

            if (!partnerUsers.Any())
            { 
                return NotFound();
            }

            var partners = new List<Partner>();

            foreach (var pu in partnerUsers)
            {
                var partner = await partnerRepository.GetPartner(pu.PartnerId, cancellationToken).ConfigureAwait(false);
                partners.Add(partner);
            }

            return Ok(partners);
        }

        [HttpGet("{partnerId}/{userId}")]
        public IActionResult GetPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            var partnerUser = partnerUserRepository.GetPartnerUsers(cancellationToken).FirstOrDefault(pu => pu.PartnerId == partnerId && pu.UserId == userId);

            if (partnerUser == null)
            {
                return NotFound();
            }

            return Ok(partnerUser);
        }

        [HttpGet("users/{partnerId}")]
        public async Task<IActionResult> GetUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partnerUsers = partnerUserRepository.GetPartnerUsers(cancellationToken).Where(pu => pu.PartnerId == partnerId).ToList();

            if (partnerUsers == null || !partnerUsers.Any())
            {
                return NotFound();
            }

            var users = new List<DisplayUser>();

            foreach (var pu in partnerUsers)
            {
                var user = await userRepository.GetUserByInternalId(pu.UserId, cancellationToken).ConfigureAwait(false);
                users.Add(user.ToDisplayUser());
            }

            return Ok(users);
        }

        [HttpPost("{partnerId}/{userId}")]

        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId)
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

            var partnerUser = new PartnerUser()
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = currentUser.Id,
                LastUpdatedByUserId = currentUser.Id
            };

            await partnerUserRepository.AddPartnerUser(partnerUser).ConfigureAwait(false);

            return CreatedAtAction(nameof(GetPartnerUser), new { partnerId, userId });
        }
    }
}
