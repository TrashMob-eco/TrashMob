namespace TrashMob.Controllers.V2
{
    using System;
    using System.Collections.Generic;
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
    using TrashMob.Models.Poco;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing dependent invitations.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/dependentinvitations")]
    public class DependentInvitationsV2Controller(
        IDependentInvitationManager dependentInvitationManager,
        IDependentManager dependentManager,
        ILogger<DependentInvitationsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Verifies an invitation token and returns invite details. No authentication required.
        /// </summary>
        /// <param name="token">The invitation token.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the invitation details.</response>
        [HttpGet("verify")]
        [AllowAnonymous]
        [ProducesResponseType(typeof(DependentInvitationInfo), StatusCodes.Status200OK)]
        public async Task<IActionResult> VerifyToken([FromQuery] string token, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 VerifyInvitationToken");

            var info = await dependentInvitationManager.VerifyTokenAsync(token, cancellationToken);
            return Ok(info);
        }

        /// <summary>
        /// Gets all invitations for a specific dependent.
        /// </summary>
        /// <param name="userId">The parent user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the invitations.</response>
        /// <response code="403">User is not the parent.</response>
        [HttpGet("users/{userId}/dependents/{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<DependentInvitation>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetInvitationsForDependent(
            Guid userId, Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetInvitationsForDependent User={UserId}, Dependent={DependentId}", userId, dependentId);

            if (userId != UserId)
            {
                return Forbid();
            }

            var invitations = await dependentInvitationManager.GetByDependentIdAsync(dependentId, cancellationToken);
            return Ok(invitations);
        }

        /// <summary>
        /// Creates and sends an invitation for a dependent to create their own account.
        /// </summary>
        /// <param name="userId">The parent user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="request">The invitation request containing the email.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Invitation created.</response>
        /// <response code="400">Invalid request.</response>
        /// <response code="403">User is not the parent.</response>
        /// <response code="404">Dependent not found.</response>
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
            logger.LogInformation("V2 CreateInvitation User={UserId}, Dependent={DependentId}", userId, dependentId);

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

                return CreatedAtAction(nameof(GetInvitationsForDependent),
                    new { userId, dependentId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Cancels an existing invitation.
        /// </summary>
        /// <param name="invitationId">The invitation ID to cancel.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Invitation cancelled.</response>
        /// <response code="400">Invalid operation.</response>
        [HttpPost("{invitationId}/cancel")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CancelInvitation(Guid invitationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 CancelInvitation Id={InvitationId}", invitationId);

            try
            {
                await dependentInvitationManager.CancelInvitationAsync(invitationId, UserId, cancellationToken);
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
        /// <param name="invitationId">The invitation ID to resend.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated invitation.</response>
        /// <response code="400">Invalid operation.</response>
        [HttpPost("{invitationId}/resend")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentInvitation), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ResendInvitation(Guid invitationId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ResendInvitation Id={InvitationId}", invitationId);

            try
            {
                var result = await dependentInvitationManager.ResendInvitationAsync(invitationId, UserId, cancellationToken);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
