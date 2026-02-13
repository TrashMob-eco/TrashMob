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
    /// Controller for managing partners, including retrieval and update.
    /// </summary>
    [Route("api/partners")]
    public class PartnersController : KeyedController<Partner>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnersController"/> class.
        /// </summary>
        /// <param name="partnerManager">The partner manager.</param>
        public PartnersController(IKeyedManager<Partner> partnerManager)
            : base(partnerManager)
        {
        }

        /// <summary>
        /// Gets a partner by its unique identifier.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner.</remarks>
        [HttpGet("{partnerId}")]
        public async Task<IActionResult> Get(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(partnerId, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Updates a partner. Requires admin or partner user.
        /// </summary>
        /// <param name="partner">The partner to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated partner.</remarks>
        [HttpPut]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Update(Partner partner, CancellationToken cancellationToken)
        {
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await Manager.UpdateAsync(partner, UserId, cancellationToken).ConfigureAwait(false);
            TrackEvent(nameof(Update) + typeof(Partner));

            return Ok(result);
        }
    }
}