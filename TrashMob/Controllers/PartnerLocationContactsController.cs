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
    /// Controller for managing partner location contacts, including retrieval and creation.
    /// </summary>
    [Authorize]
    [Route("api/partnerlocationcontacts")]
    public class PartnerLocationContactsController : SecureController
    {
        private readonly IPartnerLocationContactManager partnerLocationContactManager;
        private readonly IPartnerLocationManager partnerLocationManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationContactsController"/> class.
        /// </summary>
        /// <param name="partnerLocationManager">The partner location manager.</param>
        /// <param name="partnerLocationContactManager">The partner location contact manager.</param>
        public PartnerLocationContactsController(IPartnerLocationManager partnerLocationManager,
            IPartnerLocationContactManager partnerLocationContactManager)
        {
            this.partnerLocationManager = partnerLocationManager;
            this.partnerLocationContactManager = partnerLocationContactManager;
        }

        /// <summary>
        /// Gets all contacts for a given partner location. Requires a valid user.
        /// </summary>
        /// <param name="partnerLocationId">The partner location ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner location contacts.</remarks>
        [HttpGet("getbypartnerlocation/{partnerLocationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetByPartnerLocation(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partnerLocationServices =
                await partnerLocationContactManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        /// <summary>
        /// Gets a partner location contact by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="partnerLocationContactId">The partner location contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner location contact.</remarks>
        [HttpGet("{partnerLocationContactId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerLocationContactId, CancellationToken cancellationToken)
        {
            var partnerLocationContact =
                await partnerLocationContactManager.GetAsync(partnerLocationContactId, cancellationToken);

            return Ok(partnerLocationContact);
        }

        /// <summary>
        /// Adds a new partner location contact.
        /// </summary>
        /// <param name="partnerLocationContact">The partner location contact to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created partner location contact.</remarks>
        [HttpPost]
        public async Task<IActionResult> AddPartnerLocationContact(PartnerLocationContact partnerLocationContact,
            CancellationToken cancellationToken = default)
        {
            var partner =
                await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationContact.PartnerLocationId,
                    cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationContactManager.AddAsync(partnerLocationContact, UserId, cancellationToken);
            TrackEvent(nameof(AddPartnerLocationContact));

            return Ok();
        }

        /// <summary>
        /// Updates an existing partner location contact.
        /// </summary>
        /// <param name="partnerLocationContact">The partner location contact to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated partner location contact.</remarks>
        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocationContact(PartnerLocationContact partnerLocationContact,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner =
                await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationContact.PartnerLocationId,
                    cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerLocationContactManager
                .UpdateAsync(partnerLocationContact, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerLocationContact));

            return Ok(result);
        }

        /// <summary>
        /// Deletes a partner location contact.
        /// </summary>
        /// <param name="partnerLocationContactId">The partner location contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted partner location contact.</remarks>
        [HttpDelete("{partnerLocationContactId}")]
        public async Task<IActionResult> DeletePartnerLocationContact(Guid partnerLocationContactId,
            CancellationToken cancellationToken)
        {
            var partner =
                await partnerLocationContactManager.GetPartnerForLocationContact(partnerLocationContactId,
                    cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerLocationContactManager.DeleteAsync(partnerLocationContactId, cancellationToken);
            TrackEvent(nameof(DeletePartnerLocationContact));

            return NoContent();
        }
    }
}