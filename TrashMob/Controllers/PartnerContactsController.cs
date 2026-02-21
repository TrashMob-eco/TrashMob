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
    /// Controller for managing partner contacts, including retrieval and creation.
    /// </summary>
    [Authorize]
    [Route("api/partnercontacts")]
    public class PartnerContactsController(
        IKeyedManager<Partner> partnerManager,
        IPartnerContactManager partnerContactManager)
        : SecureController
    {

        /// <summary>
        /// Gets all contacts for a given partner. Requires a valid user.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner contacts.</remarks>
        [HttpGet("getbypartner/{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            var partnerContacts = await partnerContactManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(partnerContacts);
        }

        /// <summary>
        /// Gets a partner contact by its unique identifier. Requires a valid user.
        /// </summary>
        /// <param name="partnerContactId">The partner contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner contact.</remarks>
        [HttpGet("{partnerContactId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerContactId, CancellationToken cancellationToken)
        {
            var partnerContact = await partnerContactManager.GetAsync(partnerContactId, cancellationToken);

            return Ok(partnerContact);
        }

        /// <summary>
        /// Adds a new partner contact.
        /// </summary>
        /// <param name="partnerContact">The partner contact to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The newly created partner contact.</remarks>
        [HttpPost]
        public async Task<IActionResult> AddPartnerContact(PartnerContact partnerContact,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerContact.PartnerId, cancellationToken);

            if (partner is null)
            {
                return NotFound();
            }

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerContactManager.AddAsync(partnerContact, UserId, cancellationToken);
            TrackEvent(nameof(AddPartnerContact));

            return Ok();
        }

        /// <summary>
        /// Updates an existing partner contact.
        /// </summary>
        /// <param name="partnerContact">The partner contact to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The updated partner contact.</remarks>
        [HttpPut]
        public async Task<IActionResult> UpdatePartnerContact(PartnerContact partnerContact,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerContact.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerContactManager.UpdateAsync(partnerContact, UserId, cancellationToken);
            TrackEvent(nameof(UpdatePartnerContact));

            return Ok(result);
        }

        /// <summary>
        /// Deletes a partner contact.
        /// </summary>
        /// <param name="partnerContactId">The partner contact ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The ID of the deleted partner contact.</remarks>
        [HttpDelete("{partnerContactId}")]
        public async Task<IActionResult> DeletePartnerContact(Guid partnerContactId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerContactManager.GetPartnerForContact(partnerContactId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerContactManager.DeleteAsync(partnerContactId, cancellationToken);
            TrackEvent(nameof(DeletePartnerContact));

            return NoContent();
        }
    }
}