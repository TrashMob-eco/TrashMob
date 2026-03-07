namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Identity.Web.Resource;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Manages dependent invitations — inviting minors to create their own TrashMob accounts.
    /// </summary>
    [Route("api/dependentinvitations")]
    public class DependentInvitationsController(
        IDependentInvitationManager dependentInvitationManager,
        IDependentManager dependentManager)
        : SecureController
    {
        /// <summary>
        /// Verifies an invitation token and returns invite details. No authentication required.
        /// </summary>
        [HttpGet("verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DependentInvitationInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyToken([FromQuery] string token, CancellationToken cancellationToken)
        {
            var info = await dependentInvitationManager.VerifyTokenAsync(token, cancellationToken);
            return Ok(info);
        }

        /// <summary>
        /// Creates and sends an invitation for a dependent to create their own account.
        /// </summary>
        [HttpPost("users/{userId}/dependents/{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentInvitation), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateInvitation(
            Guid userId, Guid dependentId, [FromBody] CreateDependentInvitationRequest request,
            CancellationToken cancellationToken)
        {
            if (userId != UserId)
            {
                return Forbid();
            }

            var dependent = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (dependent == null || dependent.ParentUserId != userId)
            {
                return NotFound();
            }

            try
            {
                var result = await dependentInvitationManager.CreateInvitationAsync(
                    dependentId, request.Email, userId, cancellationToken);
                TrackEvent(nameof(CreateInvitation));

                return CreatedAtAction(nameof(GetInvitationsForDependent),
                    new { userId, dependentId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Gets all invitations for a specific dependent.
        /// </summary>
        [HttpGet("users/{userId}/dependents/{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<DependentInvitation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInvitationsForDependent(
            Guid userId, Guid dependentId, CancellationToken cancellationToken)
        {
            if (userId != UserId)
            {
                return Forbid();
            }

            var invitations = await dependentInvitationManager.GetByDependentIdAsync(dependentId, cancellationToken);
            return Ok(invitations);
        }

        /// <summary>
        /// Cancels an existing invitation.
        /// </summary>
        [HttpPost("{invitationId}/cancel")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CancelInvitation(Guid invitationId, CancellationToken cancellationToken)
        {
            try
            {
                await dependentInvitationManager.CancelInvitationAsync(invitationId, UserId, cancellationToken);
                TrackEvent(nameof(CancelInvitation));
                return Ok();
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Resends an invitation with a new token.
        /// </summary>
        [HttpPost("{invitationId}/resend")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentInvitation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ResendInvitation(Guid invitationId, CancellationToken cancellationToken)
        {
            try
            {
                var result = await dependentInvitationManager.ResendInvitationAsync(invitationId, UserId, cancellationToken);
                TrackEvent(nameof(ResendInvitation));
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
