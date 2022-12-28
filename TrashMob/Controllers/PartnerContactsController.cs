namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnercontacts")]
    public class PartnerContactsController : SecureController
    {
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IPartnerContactManager partnerContactManager;

        public PartnerContactsController(IKeyedManager<Partner> partnerManager,
                                         IPartnerContactManager partnerContactManager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.partnerContactManager = partnerContactManager;            
        }

        [HttpGet("getbypartner/{partnerId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            var partnerContacts = await partnerContactManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(partnerContacts);
        }

        [HttpGet("{partnerContactId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> Get(Guid partnerContactId, CancellationToken cancellationToken)
        {
            var partnerContact = await partnerContactManager.GetAsync(partnerContactId, cancellationToken);

            return Ok(partnerContact);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerContact(PartnerContact partnerContact, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerContact.PartnerId, cancellationToken);
            
            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerContactManager.AddAsync(partnerContact, UserId, cancellationToken);
            TelemetryClient.TrackEvent(nameof(AddPartnerContact));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerContact(PartnerContact partnerContact, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerContact.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerContactManager.UpdateAsync(partnerContact, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerContact));

            return Ok(result);
        }

        [HttpDelete("{partnerContactId}")]
        public async Task<IActionResult> DeletePartnerContact(Guid partnerContactId, CancellationToken cancellationToken)
        {
            var partner = await partnerContactManager.GetPartnerForContact(partnerContactId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerContactManager.DeleteAsync(partnerContactId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerContact));

            return Ok(partnerContactId);
        }
    }
}
