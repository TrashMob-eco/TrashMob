namespace TrashMob.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for managing partner requests, including creation, approval, and denial.
    /// </summary>
    [Route("api/partnerrequests")]
    public class PartnerRequestsController : SecureController
    {
        private readonly IPartnerRequestManager partnerRequestManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerRequestsController"/> class.
        /// </summary>
        /// <param name="partnerRequestManager">The partner request manager.</param>
        public PartnerRequestsController(IPartnerRequestManager partnerRequestManager)
        {
            this.partnerRequestManager = partnerRequestManager;
        }

        /// <summary>
        /// Adds a new partner request. Requires a valid user.
        /// </summary>
        /// <param name="partnerRequest">The partner request to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns OK if successful.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddPartnerRequest(PartnerRequest partnerRequest,
            CancellationToken cancellationToken)
        {
            await partnerRequestManager.AddAsync(partnerRequest, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(AddPartnerRequest));

            return Ok();
        }

        /// <summary>
        /// Approves a partner request. Admin only.
        /// </summary>
        /// <param name="partnerRequestId">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns OK if successful.</remarks>
        [HttpPut("approve/{partnerRequestId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> ApprovePartnerRequest(Guid partnerRequestId,
            CancellationToken cancellationToken)
        {
            var partnerRequest = await partnerRequestManager
                .ApproveBecomeAPartnerAsync(partnerRequestId, UserId, cancellationToken).ConfigureAwait(false);
            TelemetryClient.TrackEvent(nameof(ApprovePartnerRequest));

            return Ok();
        }

        /// <summary>
        /// Denies a partner request. Admin only.
        /// </summary>
        /// <param name="partnerRequestId">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns OK if successful.</remarks>
        [HttpPut("deny/{partnerRequestId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(200)]
        public async Task<IActionResult> DenyPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            var partnerRequest = await partnerRequestManager
                .DenyBecomeAPartnerAsync(partnerRequestId, UserId, cancellationToken).ConfigureAwait(false);

            TelemetryClient.TrackEvent(nameof(DenyPartnerRequest));

            return Ok();
        }

        /// <summary>
        /// Gets all partner requests. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns a list of partner requests.</remarks>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(typeof(IEnumerable<PartnerRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerRequests(CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetAsync(cancellationToken));
        }

        /// <summary>
        /// Gets a partner request by its unique identifier. Admin only.
        /// </summary>
        /// <param name="partnerRequestId">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns the partner request.</remarks>
        [HttpGet("{partnerRequestId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(typeof(PartnerRequest), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerRequest(Guid partnerRequestId, CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetAsync(partnerRequestId, cancellationToken).ConfigureAwait(false));
        }

        /// <summary>
        /// Gets partner requests by user ID. Requires a valid user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Returns a list of partner requests for the user.</remarks>
        [HttpGet("byuserid/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerRequest>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPartnerRequestsByUser(Guid userId, CancellationToken cancellationToken)
        {
            return Ok(await partnerRequestManager.GetByCreatedUserIdAsync(userId, cancellationToken)
                .ConfigureAwait(false));
        }
    }
}