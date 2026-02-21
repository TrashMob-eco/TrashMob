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

    /// <summary>
    /// Controller for managing partner social media accounts, including retrieval and creation.
    /// </summary>
    [Authorize]
    [Route("api/partnersocialmediaaccounts")]
    public class PartnerSocialMediaAccountController(
        IPartnerSocialMediaAccountManager manager,
        IKeyedManager<Partner> partnerManager)
        : SecureController
    {

        /// <summary>
        /// Gets all social media accounts for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner social media accounts.</remarks>
        [HttpGet("getbypartner/{partnerId}")]
        public async Task<IActionResult> GetPartnerSocialMediaAccounts(Guid partnerId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var socialMediaAccounts = await manager.GetByParentIdAsync(partnerId, cancellationToken);
            return Ok(socialMediaAccounts);
        }

        /// <summary>
        /// Gets a partner social media account by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="partnerSocialMediaAccountId">The partner social media account ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner social media account.</remarks>
        [HttpGet("{partnerSocialMediaAccountId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerSocialMediaAccountId, CancellationToken cancellationToken)
        {
            var partnerContact = await manager.GetAsync(partnerSocialMediaAccountId, cancellationToken);

            return Ok(partnerContact);
        }

        /// <summary>
        /// Adds a new partner social media account.
        /// </summary>
        /// <param name="partnerSocialMediaAccount">The partner social media account to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPost]
        public async Task<IActionResult> AddPartnerSocialMediaAccount(
            PartnerSocialMediaAccount partnerSocialMediaAccount, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerSocialMediaAccount.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await manager.AddAsync(partnerSocialMediaAccount, UserId, cancellationToken);
            TrackEvent(nameof(AddPartnerSocialMediaAccount));

            return Ok();
        }

        /// <summary>
        /// Updates an existing partner social media account.
        /// </summary>
        /// <param name="partnerSocialMediaAccount">The partner social media account to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpPut]
        public async Task<IActionResult> UpdatePartnerSocialMediaAccount(
            PartnerSocialMediaAccount partnerSocialMediaAccount, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerSocialMediaAccount.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await manager.UpdateAsync(partnerSocialMediaAccount, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerSocialMediaAccount));

            return Ok(result);
        }

        /// <summary>
        /// Deletes a partner social media account.
        /// </summary>
        /// <param name="partnerSocialMediaAccountId">The partner social media account ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Action result.</remarks>
        [HttpDelete("{partnerSocialMediaAccountId}")]
        public async Task<IActionResult> DeletePartnerSocialMediaAccount(Guid partnerSocialMediaAccountId,
            CancellationToken cancellationToken)
        {
            var partner = await manager.GetPartnerForSocialMediaAccount(partnerSocialMediaAccountId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await manager.DeleteAsync(partnerSocialMediaAccountId, cancellationToken);
            TrackEvent(nameof(DeletePartnerSocialMediaAccount));

            return NoContent();
        }
    }
}