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
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for community admin adoption management.
    /// </summary>
    [Route("api/communities/{partnerId}/adoptions")]
    [ApiController]
    public class CommunityAdoptionsController : SecureController
    {
        private readonly ITeamAdoptionManager adoptionManager;
        private readonly IKeyedManager<Partner> partnerManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityAdoptionsController"/> class.
        /// </summary>
        /// <param name="adoptionManager">The team adoption manager.</param>
        /// <param name="partnerManager">The partner manager.</param>
        public CommunityAdoptionsController(
            ITeamAdoptionManager adoptionManager,
            IKeyedManager<Partner> partnerManager)
        {
            this.adoptionManager = adoptionManager;
            this.partnerManager = partnerManager;
        }

        /// <summary>
        /// Gets all pending adoption applications for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("pending")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPendingApplications(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            // Check authorization - must be partner admin
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var applications = await adoptionManager.GetPendingByCommunityAsync(partnerId, cancellationToken);
            return Ok(applications);
        }

        /// <summary>
        /// Gets all approved adoptions for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("approved")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<TeamAdoption>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetApprovedAdoptions(Guid partnerId, CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            // Check authorization
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var adoptions = await adoptionManager.GetApprovedByCommunityAsync(partnerId, cancellationToken);
            return Ok(adoptions);
        }

        /// <summary>
        /// Approves an adoption application. Only community admins can approve.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{adoptionId}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoption), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> ApproveApplication(
            Guid partnerId,
            Guid adoptionId,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            // Check authorization
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await adoptionManager.ApproveApplicationAsync(adoptionId, UserId, cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }

        /// <summary>
        /// Rejects an adoption application. Only community admins can reject.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="adoptionId">The adoption application ID.</param>
        /// <param name="request">The rejection request with reason.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{adoptionId}/reject")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(TeamAdoption), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RejectApplication(
            Guid partnerId,
            Guid adoptionId,
            [FromBody] RejectAdoptionRequest request,
            CancellationToken cancellationToken)
        {
            var partner = await partnerManager.GetAsync(partnerId, cancellationToken);
            if (partner == null)
            {
                return NotFound();
            }

            // Check authorization
            var authResult = await AuthorizationService.AuthorizeAsync(
                User, partner, AuthorizationPolicyConstants.UserIsPartnerUserOrIsAdmin);
            if (!authResult.Succeeded)
            {
                return Forbid();
            }

            var result = await adoptionManager.RejectApplicationAsync(
                adoptionId,
                request.RejectionReason,
                UserId,
                cancellationToken);

            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }

            return Ok(result.Data);
        }
    }

    /// <summary>
    /// Request model for rejecting an adoption application.
    /// </summary>
    public class RejectAdoptionRequest
    {
        /// <summary>
        /// Gets or sets the reason for rejection.
        /// </summary>
        public string RejectionReason { get; set; }
    }
}
