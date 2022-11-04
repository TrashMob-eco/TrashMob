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

    [Authorize]
    [Route("api/partneradmins")]
    public class PartnerAdminsController : SecureController
    {
        private readonly IBaseManager<PartnerAdmin> partnerAdminManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<User> userManager;

        public PartnerAdminsController(IKeyedManager<User> userManager,
                                       IBaseManager<PartnerAdmin> partnerAdminManager, 
                                       IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerAdminManager = partnerAdminManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerUsers(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken));
        }

        [HttpGet("getpartnersforuser/{userId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnersForUser(Guid userId, CancellationToken cancellationToken)
        {
            // Todo fix this
            var partnerUsers = (await partnerAdminManager.GetAsync(cancellationToken)).Where(pu => pu.UserId == userId).ToList();

            if (!partnerUsers.Any())
            { 
                return NotFound();
            }

            var partners = new List<Partner>();

            foreach (var pu in partnerUsers)
            {
                var partner = await partnerManager.GetAsync(pu.PartnerId, cancellationToken).ConfigureAwait(false);
                partners.Add(partner);
            }

            return Ok(partners);
        }

        [HttpGet("{partnerId}/{userId}")]
        public async Task<IActionResult> GetPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser = (await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken)).FirstOrDefault(pu => pu.UserId == userId);

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
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUsers = await partnerAdminManager.GetByParentIdAsync(partnerId, cancellationToken);

            if (partnerUsers == null || !partnerUsers.Any())
            {
                return NotFound();
            }

            var users = new List<DisplayUser>();

            foreach (var pu in partnerUsers)
            {
                var user = await userManager.GetAsync(pu.UserId, cancellationToken).ConfigureAwait(false);
                users.Add(user.ToDisplayUser());
            }

            return Ok(users);
        }

        [HttpPost("{partnerId}/{userId}")]

        public async Task<IActionResult> AddPartnerUser(Guid partnerId, Guid userId, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerUser = new PartnerAdmin()
            {
                PartnerId = partnerId,
                UserId = userId,
                CreatedByUserId = UserId,
                LastUpdatedByUserId = UserId
            };

            var result = await partnerAdminManager.AddAsync(partnerUser, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerUser));

            return CreatedAtAction(nameof(GetPartnerUser), new { partnerId, userId }, result);
        }
    }
}
