namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnersocialmediaaccounts")]
    public class PartnerSocialMediaAccountController : SecureController
    {
        private readonly IKeyedManager<PartnerSocialMediaAccount> manager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerSocialMediaAccountController(IKeyedManager<PartnerSocialMediaAccount> manager,
                                                   IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.manager = manager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerSocialMediaAccounts(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var socialMediaAccounts = await manager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(socialMediaAccounts);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerSocialMediaAccount(PartnerSocialMediaAccount partnerSocialMediaAccount, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerSocialMediaAccount.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.AddAsync(partnerSocialMediaAccount, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerSocialMediaAccount));

            return Ok();
        }
    }
}
