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

    /// <summary>
    /// Controller for managing partner admin invitations, including retrieval and creation.
    /// </summary>
    [Authorize]
    [Route("api/partneradmininvitations")]
    public class PartnerAdminInvitationsController(
        IKeyedManager<User> userManager,
        IPartnerAdminInvitationManager partnerAdminInvitationManager,
        IKeyedManager<Partner> partnerManager)
        : SecureController
    {

        /// <summary>
        /// Gets all partner admin invitations for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner admin invitations.</remarks>
        [HttpGet("{partnerId}")]
        public async Task<IActionResult> GetPartnerAdminInvitations(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            return Ok(await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken));
        }

        /// <summary>
        /// Gets all partner admin invitations for a given user. Requires a valid user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of partner admin invitations for the user.</remarks>
        [HttpGet("getbyuser/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> GetPartnerInvitationsForUser(Guid userId, CancellationToken cancellationToken)
        {
            var results = await partnerAdminInvitationManager.GetInvitationsForUser(userId, cancellationToken);
            return Ok(results);
        }

        /// <summary>
        /// Gets a specific partner admin invitation by partner ID and email.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="email">The email of the invited admin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>The partner admin invitation if found, otherwise NotFound.</remarks>
        [HttpGet("{partnerId}/{email}")]
        public async Task<IActionResult> GetPartnerAdminInvite(Guid partnerId, string email,
            CancellationToken cancellationToken = default)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
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

        /// <summary>
        /// Adds a new partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitation">The partner admin invitation to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Created partner admin invitation.</remarks>
        [HttpPost]
        public async Task<IActionResult> AddPartnerAdminInvitation(PartnerAdminInvitation partnerAdminInvitation,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner = await partnerManager.GetAsync(partnerAdminInvitation.PartnerId, cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
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

            var result = await partnerAdminInvitationManager.AddAsync(partnerAdminInvitation, UserId, cancellationToken);
            TrackEvent(nameof(AddPartnerAdminInvitation));

            return CreatedAtAction(nameof(GetPartnerAdminInvite),
                new { partnerAdminInvitation.PartnerId, partnerAdminInvitation.Email }, result);
        }

        /// <summary>
        /// Resends an existing partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Updated partner admin invitation.</remarks>
        [HttpPost("resend/{partnerAdminInvitationId}")]
        public async Task<IActionResult> ResendPartnerAdminInvitationAsync(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            // Make sure the person adding the user is either an admin or already a user for the partner
            var partner =
                await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId,
                    cancellationToken);
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager
                .ResendPartnerAdminInvitationAsync(partnerAdminInvitationId, UserId, cancellationToken);
            TrackEvent(nameof(ResendPartnerAdminInvitationAsync));

            return Ok(result);
        }

        /// <summary>
        /// Accepts a partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>OK if successful.</remarks>
        [HttpPost("accept/{partnerAdminInvitationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> AcceptPartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            await partnerAdminInvitationManager.AcceptInvitationAsync(partnerAdminInvitationId, UserId, cancellationToken);
            TrackEvent(nameof(AcceptPartnerAdminInvitation));

            return Ok();
        }

        /// <summary>
        /// Declines a partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>OK if successful.</remarks>
        [HttpPost("decline/{partnerAdminInvitationId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public async Task<IActionResult> DeclinePartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            await partnerAdminInvitationManager.DeclineInvitationAsync(partnerAdminInvitationId, UserId, cancellationToken);
            TrackEvent(nameof(AcceptPartnerAdminInvitation));

            return Ok();
        }

        /// <summary>
        /// Deletes a partner admin invitation.
        /// </summary>
        /// <param name="partnerAdminInvitationId">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>OK if successful.</remarks>
        [HttpDelete("{partnerAdminInvitationId}")]
        public async Task<IActionResult> DeletePartnerAdminInvitation(Guid partnerAdminInvitationId,
            CancellationToken cancellationToken)
        {
            var partner =
                await partnerAdminInvitationManager.GetPartnerForInvitation(partnerAdminInvitationId,
                    cancellationToken);

            // Make sure the person adding the user is either an admin or already a user for the partner
            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerAdminInvitationManager.DeleteAsync(partnerAdminInvitationId, cancellationToken);
            TrackEvent(nameof(AddPartnerAdminInvitation));

            return NoContent();
        }
    }
}