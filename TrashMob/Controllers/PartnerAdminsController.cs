namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Poco;

    [Authorize]
    [Route("api/partneradmins")]
    public class PartnerAdminsController : SecureController
    {
        private readonly IPartnerAdminManager partnerAdminManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<User> userManager;

        public PartnerAdminsController(IKeyedManager<User> userManager,
            IPartnerAdminManager partnerAdminManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerAdminManager = partnerAdminManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerAdmins(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerAdminManager.GetAdminsForPartnerAsync(partnerId, cancellationToken));
        }

        [HttpGet("getpartnersforuser/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            var partners = await partnerAdminManager.GetPartnersByUserIdAsync(userId, cancellationToken);
            return Ok(partners);
        }

        [HttpGet("{partnerId}/{userId}")]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser =
                (await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken)).FirstOrDefault(pu =>
                    pu.UserId == userId);

            if (partnerUser == null)
            {
                return NotFound();
            }

            return Ok(partnerUser);
        }

        [HttpGet("users/{partnerId}")]
        public async Task<IActionResult> GetUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerAdmins = await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken);

            if (partnerAdmins == null || !partnerAdmins.Any())
            {
                return NotFound();
            }

            var users = new List<DisplayUser>();

            foreach (var pu in partnerAdmins)
            {
                var user = await userManager.GetAsync(pu.UserId, cancellationToken).ConfigureAwait(false);
                users.Add(user.ToDisplayUser());
            }

            return Ok(users);
        }

        [HttpPost("{partnerId}/{userId}")]
        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            if (userId == Guid.Empty)
            {
                return BadRequest("UserId is required.");
            }

            var partnerUser = new PartnerAdmin
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = UserId,
                LastUpdatedByUserId = UserId,
            };

            var result = await partnerAdminManager.AddAsync(partnerUser, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerUser));

            return CreatedAtAction(nameof(GetPartnerUser), new { partnerId, userId }, result);
        }
    }
}