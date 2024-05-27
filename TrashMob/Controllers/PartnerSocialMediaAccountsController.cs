namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnersocialmediaaccounts")]
    public class PartnerSocialMediaAccountController : SecureController
    {
        private readonly IPartnerSocialMediaAccountManager manager;
        private readonly IKeyedManager<Partner> partnerManager;

        public PartnerSocialMediaAccountController(IPartnerSocialMediaAccountManager partnerSocialMediaAccountManager,
            IKeyedManager<Partner> partnerManager)
        {
            manager = partnerSocialMediaAccountManager;
            this.partnerManager = partnerManager;
        }

        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerSocialMediaAccounts(Guid partnerId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var socialMediaAccounts = await manager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(socialMediaAccounts);
        }

        [HttpGet("{partnerSocialMediaAccountId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerSocialMediaAccountId, CancellationToken cancellationToken)
        {
            var partnerContact = await manager.GetAsync(partnerSocialMediaAccountId, cancellationToken);

            return Ok(partnerContact);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerSocialMediaAccount(
            PartnerSocialMediaAccount partnerSocialMediaAccount, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerSocialMediaAccount.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.AddAsync(partnerSocialMediaAccount, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerSocialMediaAccount));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerSocialMediaAccount(
            PartnerSocialMediaAccount partnerSocialMediaAccount, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerSocialMediaAccount.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await manager.UpdateAsync(partnerSocialMediaAccount, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerSocialMediaAccount));

            return Ok(result);
        }

        [HttpDelete("{partnerSocialMediaAccountId}")]
        public async Task<IActionResult> DeletePartnerSocialMediaAccount(Guid partnerSocialMediaAccountId,
            CancellationToken cancellationToken)
        {
            var partner = await manager.GetPartnerForSocialMediaAccount(partnerSocialMediaAccountId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await manager.DeleteAsync(partnerSocialMediaAccountId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerSocialMediaAccount));

            return Ok(partnerSocialMediaAccountId);
        }
    }
}