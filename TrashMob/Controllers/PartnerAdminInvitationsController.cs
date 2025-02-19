namespace TrashMob.Controllers
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
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
        {
            this.partnerManager = partnerManager;
            this.userManager = userManager;
            this.partnerAdminInvitationManager = partnerAdminInvitationManager;
        }

        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerAdminInvitations(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            return Ok(await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken));
        }

        [HttpGet("getbyuser/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnerInvitationsForUser(Guid userId, CancellationToken cancellationToken)
        {
            var results = await partnerAdminInvitationManager.GetInvitationsForUser(userId, cancellationToken);
            return Ok(results);
        }

        [HttpGet("{partnerId}/{email}")]
        public async Task<IActionResult> GetPartnerAdminInvite(Guid partnerId, string email,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var partnerInvite =
                (await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken)).FirstOrDefault(
                    pu => pu.Email == email);

            if (partnerInvite == null)
            {
                return NotFound();
            }

            return Ok(partnerInvite);
        }

        [HttpPost]
        public async Task<IActionResult> AddPartnerAdminInvitation(PartnerAdminInvitation partnerAdminInvitation,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerAdminInvitation.PartnerId, cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            if (partnerAdminInvitation.PartnerId == Guid.Empty)
            {
                return BadRequest("PartnerId is required.");
            }

            if (string.IsNullOrWhiteSpace(partnerAdminInvitation.Email))
            {
                return BadRequest("Email is required.");
            }

            var addUser = await userManager.GetAsync(p => p.Email == partnerAdminInvitation.Email, cancellationToken);

            if (addUser == null) 
            {
                return NotFound($"User with Email {partnerAdminInvitation.Email} not found.");
            }

            var result = await partnerAdminInvitationManager.AddAsync(partnerAdminInvitation, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerAdminInvitation));

            return CreatedAtAction(nameof(GetPartnerAdminInvite),
                new { partnerAdminInvitation.PartnerId, partnerAdminInvitation.Email }, result);
        }

        [HttpPost("resend/{partnerAdminInvitationId}")]
        public async Task<IActionResult> ResendPartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner =
                await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId,
                    cancellationToken);
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager
                .ResendPartnerAdminInvitation(partnerAdminInvitationId, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ResendPartnerAdminInvitation));

            return Ok(result);
        }

        [HttpPost("accept/{partnerAdminInvitationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> AcceptPartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            await partnerAdminInvitationManager.AcceptInvitation(partnerAdminInvitationId, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AcceptPartnerAdminInvitation));

            return Ok();
        }

        [HttpPost("decline/{partnerAdminInvitationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> DeclinePartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            await partnerAdminInvitationManager.DeclineInvitation(partnerAdminInvitationId, UserId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AcceptPartnerAdminInvitation));

            return Ok();
        }

        [HttpDelete("{partnerAdminInvitationId}")]
        public async Task<IActionResult> DeletePartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            var partner =
                await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId,
                    cancellationToken);

            // Make sure the person adding the user is either an admin or already a user for the partner
            var authResult = await AuthorizationService.AuthorizeAsync(User, partner,
                AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager.DeleteAsync(partnerAdminInvitationId, cancellationToken)
                .ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerAdminInvitation));

            return Ok(result);
        }
    }
}