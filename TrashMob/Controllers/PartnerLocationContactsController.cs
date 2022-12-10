namespace TrashMob.Controllers
{
    using Microsoft.ApplicationInsights;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Migrations;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partnerlocationcontacts")]
    public class PartnerLocationContactsController : SecureController
    {
        private readonly IPartnerLocationManager partnerLocationManager;
        private readonly IPartnerLocationContactManager partnerLocationContactManager;

        public PartnerLocationContactsController(IPartnerLocationManager partnerLocationManager,
                                                 IPartnerLocationContactManager partnerLocationContactManager)
            : base()
        {
            this.partnerLocationManager = partnerLocationManager;
            this.partnerLocationContactManager = partnerLocationContactManager;            
        }

        [HttpGet("getbypartnerlocation/{partnerLocationId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetByPartnerLocation(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocationServices = await partnerLocationContactManager.GetByParentIdAsync(partnerLocationId, cancellationToken);

            return Ok(partnerLocationServices);
        }

        [HttpGet("{partnerLocationContactId}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> Get(Guid partnerLocationContactId, CancellationToken cancellationToken)
        {
            var partnerLocationContact = await partnerLocationContactManager.GetAsync(partnerLocationContactId, cancellationToken);

            return Ok(partnerLocationContact);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerLocationContact(PartnerLocationContact partnerLocationContact, CancellationToken cancellationToken = default)
        {
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationContact.PartnerLocationId, cancellationToken);

            if (partner == null)
            {
                return NotFound();
            }

            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationContactManager.AddAsync(partnerLocationContact, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerLocationContact));

            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerLocationContact(PartnerLocationContact partnerLocationContact, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerLocationManager.GetPartnerForLocationAsync(partnerLocationContact.PartnerLocationId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerLocationContactManager.UpdateAsync(partnerLocationContact, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(UpdatePartnerLocationContact));

            return Ok(result);
        }

        [HttpDelete("{partnerLocationContactId}")]
        public async Task<IActionResult> DeletePartnerLocationContact(Guid partnerLocationContactId, CancellationToken cancellationToken)
        {
            var partner = await partnerLocationContactManager.GetPartnerForLocationContact(partnerLocationContactId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            await partnerLocationContactManager.DeleteAsync(partnerLocationContactId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(DeletePartnerLocationContact));

            return Ok(partnerLocationContactId);
        }
    }
}
