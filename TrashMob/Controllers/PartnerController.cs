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
    public class PartnersController(IKeyedManager<Partner> partnerManager)
        : KeyedController<Partner>(partnerManager)
    {

        /// <summary>
        /// Gets a partner by its unique identifier.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner.</remarks>
        [HttpGet("{partnerId}")]
        public async Task<IActionResult> Get(Guid partnerId, CancellationToken cancellationToken)
        {
            return Ok(await Manager.GetAsync(partnerId, cancellationToken));
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
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await Manager.UpdateAsync(partner, UserId, cancellationToken);
            TrackEvent(nameof(Update) + typeof(Partner));

            return Ok(result);
        }
    }
}