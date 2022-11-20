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
    using TrashMob.Shared.Managers.Interfaces;

    [Authorize]
    [Route("api/partneradmininvitations")]
    public class PartnerAdminInvitationsController : SecureController
    {
        private readonly IPartnerAdminInvitationManager partnerAdminInvitationManager;
        private readonly IKeyedManager<Partner> partnerManager;
        private readonly IKeyedManager<User> userManager;

        public PartnerAdminInvitationsController(IKeyedManager<User> userManager,
                                                 IPartnerAdminInvitationManager partnerAdminInvitationManager,
                                                 IKeyedManager<Partner> partnerManager)
            : base()
        {
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerAdminInvitationManager = partnerAdminInvitationManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerInvitations(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken));
        }

        [HttpGet("getpartnerinvitationsforemail/{email}")]
        [Authorize(Policy = "ValidUser")]
        public async Task<IActionResult> GetPartnerInvitationsForEmail(string email, CancellationToken cancellationToken)
        {
            var partnerAdminInvitations = (await partnerAdminInvitationManager.GetAsync(cancellationToken)).Where(pu => pu.Email == email).ToList();

            if (!partnerAdminInvitations.Any())
            {
                return NotFound();
            }

            var partners = new List<Partner>();

            foreach (var pu in partnerAdminInvitations)
            {
                var partner = await partnerManager.GetAsync(pu.PartnerId, cancellationToken).ConfigureAwait(false);
                partners.Add(partner);
            }

            return Ok(partners);
        }

        [HttpGet("{partnerId}/{email}")]
        public async Task<IActionResult> GetPartnerAdminInvite(Guid partnerId, string email, CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerInvite = (await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken)).FirstOrDefault(pu => pu.Email == email);

            if (partnerInvite == null)
            {
                return NotFound();
            }

            return Ok(partnerInvite);
        }

        [HttpPost]

        public async Task<IActionResult> AddPartnerAdminInvitation(PartnerAdminInvitation partnerAdminInvitation, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerAdminInvitation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager.AddAsync(partnerAdminInvitation, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerAdminInvitation));

            return CreatedAtAction(nameof(GetPartnerAdminInvite), new { partnerAdminInvitation.PartnerId, partnerAdminInvitation.Email }, result);
        }

        [HttpPost("resend/{partnerAdminInvitationId}")]

        public async Task<IActionResult> ResendPartnerAdminInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager.ResendPartnerAdminInvitation(partnerAdminInvitationId, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ResendPartnerAdminInvitation));

            return Ok(result);
        }

        [HttpPut]
        public async Task<IActionResult> UpdatePartnerAdminInvitation(PartnerAdminInvitation partnerAdminInvitation, CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerAdminInvitation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager.UpdateAsync(partnerAdminInvitation, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerAdminInvitation));

            return Ok(result);
        }

        [HttpDelete("{partnerAdminInvitationId}")]
        public async Task<IActionResult> DeletePartnerAdminInvitation(Guid partnerAdminInvitationId, CancellationToken cancellationToken)
        {
            var partner = await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId, cancellationToken);

            // Make sure the person adding the user is either an admin or already a user for the partner
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner, "UserIsPartnerUserOrIsAdmin");

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager.DeleteAsync(partnerAdminInvitationId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerAdminInvitation));

            return Ok(result);
        }
    }
}
