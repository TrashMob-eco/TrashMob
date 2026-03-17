namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Asp.Versioning;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Cors;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing partner administrator invitations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-admin-invitations")]
    [Authorize]
    public class PartnerAdminInvitationsV2Controller(
        IKeyedManager<User> userManager,
        IPartnerAdminInvitationManager partnerAdminInvitationManager,
        IKeyedManager<Partner> partnerManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerAdminInvitationsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all partner admin invitations for a given partner.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of partner admin invitations.</returns>
        /// <response code="200">Returns the invitation list.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpGet("{partnerId}")]
        [ProducesResponseType(typeof(IEnumerable<PartnerAdminInvitationDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetByPartner(Guid partnerId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetByPartner Partner={PartnerId}", partnerId);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var invitations = await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken);

            return Ok(invitations.Select(i => i.ToV2Dto()));
        }

        /// <summary>
        /// Gets all partner admin invitations for a given user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of display partner admin invitations for the user.</returns>
        /// <response code="200">Returns the invitation list.</response>
        [HttpGet("by-user/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<DisplayPartnerAdminInvitation>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetByUser User={UserId}", userId);

            var results = await partnerAdminInvitationManager.GetInvitationsForUser(userId, cancellationToken);

            return Ok(results);
        }

        /// <summary>
        /// Gets a specific partner admin invitation by partner ID and email.
        /// </summary>
        /// <param name="partnerId">The partner ID.</param>
        /// <param name="email">The email of the invited admin.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The partner admin invitation if found.</returns>
        /// <response code="200">Returns the invitation.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">Invitation not found.</response>
        [HttpGet("{partnerId}/{email}")]
        [ProducesResponseType(typeof(PartnerAdminInvitationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetByPartnerAndEmail(Guid partnerId, string email,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetByPartnerAndEmail Partner={PartnerId} Email={Email}", partnerId, email);

            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var invitation = (await partnerAdminInvitationManager.GetByParentIdAsync(partnerId, cancellationToken))
                .FirstOrDefault(i => i.Email == email);

            if (invitation is null)
            {
                return NotFound();
            }

            return Ok(invitation.ToV2Dto());
        }

        /// <summary>
        /// Adds a new partner admin invitation.
        /// </summary>
        /// <param name="dto">The partner admin invitation to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created invitation.</returns>
        /// <response code="201">Invitation created.</response>
        /// <response code="400">Invalid request data.</response>
        /// <response code="403">User is not authorized.</response>
        /// <response code="404">User with specified email not found.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerAdminInvitationDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Add([FromBody] PartnerAdminInvitationDto dto,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerAdminInvitation Partner={PartnerId}", dto.PartnerId);

            var partner = await partnerManager.GetAsync(dto.PartnerId, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            if (dto.PartnerId == Guid.Empty)
            {
                return Problem("PartnerId is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            if (string.IsNullOrWhiteSpace(dto.Email))
            {
                return Problem("Email is required.", statusCode: StatusCodes.Status400BadRequest);
            }

            var addUser = await userManager.GetAsync(p => p.Email == dto.Email, cancellationToken);

            if (addUser is null)
            {
                return NotFound();
            }

            var entity = dto.ToEntity();
            var result = await partnerAdminInvitationManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetByPartnerAndEmail),
                new { partnerId = result.PartnerId, email = result.Email }, result.ToV2Dto());
        }

        /// <summary>
        /// Resends an existing partner admin invitation.
        /// </summary>
        /// <param name="id">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The resent invitation.</returns>
        /// <response code="200">Invitation resent.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpPost("{id}/resend")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerAdminInvitationDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Resend(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ResendPartnerAdminInvitation Invitation={InvitationId}", id);

            var partner = await partnerAdminInvitationManager.GetPartnerForInvitation(id, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            var result = await partnerAdminInvitationManager
                .ResendPartnerAdminInvitationAsync(id, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Accepts a partner admin invitation.
        /// </summary>
        /// <param name="id">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Invitation accepted.</response>
        [HttpPost("{id}/accept")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Accept(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AcceptPartnerAdminInvitation Invitation={InvitationId}", id);

            await partnerAdminInvitationManager.AcceptInvitationAsync(id, UserId, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Declines a partner admin invitation.
        /// </summary>
        /// <param name="id">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Invitation declined.</response>
        [HttpPost("{id}/decline")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Decline(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeclinePartnerAdminInvitation Invitation={InvitationId}", id);

            await partnerAdminInvitationManager.DeclineInvitationAsync(id, UserId, cancellationToken);

            return Ok();
        }

        /// <summary>
        /// Deletes a partner admin invitation.
        /// </summary>
        /// <param name="id">The partner admin invitation ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Invitation deleted.</response>
        /// <response code="403">User is not authorized.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeletePartnerAdminInvitation Invitation={InvitationId}", id);

            var partner = await partnerAdminInvitationManager.GetPartnerForInvitation(id, cancellationToken);

            if (!await IsAuthorizedAsync(partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin))
            {
                return Forbid();
            }

            await partnerAdminInvitationManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }

        private async Task<bool> IsAuthorizedAsync(object resource, string policy)
        {
            if (User.Identity?.IsAuthenticated != true)
            {
                return false;
            }

            var authResult = await authorizationService.AuthorizeAsync(User, resource, policy);
            return authResult.Succeeded;
        }
    }
}
