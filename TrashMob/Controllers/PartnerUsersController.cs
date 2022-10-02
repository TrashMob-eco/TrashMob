namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    [Authorize]
    [Route("api/partnerusers")]
    public class PartnerUsersController : SecureController
    {
        private readonly IBaseManager<PartnerUser> partnerUserManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IUserRepository userRepository;

        public PartnerUsersController(TelemetryClient telemetryClient,
                                      IUserRepository userRepository,
                                      IAuthorizationService authorizationService,
                                      IBaseManager<PartnerUser> partnerUserManager, 
                                      IKeyedManager<Partner> partnerManager)
            : base(telemetryClient, authorizationService)
        {
            this.partnerManager = partnerManager;
            this.userRepository = userRepository;
            this.partnerUserManager = partnerUserManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerUserManager.GetByParentId(partnerId, cancellationToken));
        }

        [HttpGet("getpartnersforuser/{userId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            // Todo fix this
            var partnerUsers = (await partnerUserManager.Get(cancellationToken)).Where(pu => pu.UserId == userId).ToList();

            if (!partnerUsers.Any())
            { 
                return NotFound();
            }

            var partners = new List<Partner>();

            foreach (var pu in partnerUsers)
            {
                var partner = await partnerManager.Get(pu.PartnerId, cancellationToken).ConfigureAwait(false);
                partners.Add(partner);
            }

            return Ok(partners);
        }

        [HttpGet("{partnerId}/{userId}")]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser = (await partnerUserManager.GetByParentId(partnerId, cancellationToken)).FirstOrDefault(pu => pu.UserId == userId);

            if (partnerUser == null)
            {
                return NotFound();
            }

            return Ok(partnerUser);
        }

        [HttpGet("users/{partnerId}")]
        public async Task<IActionResult> GetUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUsers = await partnerUserManager.GetByParentId(partnerId, cancellationToken);

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

        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.Get(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser = new PartnerUser()
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = UserId,
                LastUpdatedByUserId = UserId
            };

            await partnerUserManager.Add(partnerUser).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerUser));

            return CreatedAtAction(nameof(GetPartnerUser), new { partnerId, userId });
        }
    }
}
