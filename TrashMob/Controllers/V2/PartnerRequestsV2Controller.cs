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
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// V2 controller for managing partner requests, including creation, approval, and denial.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/partner-requests")]
    [Authorize]
    public class PartnerRequestsV2Controller(
        IPartnerRequestManager partnerRequestManager,
        IAuthorizationService authorizationService,
        ILogger<PartnerRequestsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Adds a new partner request.
        /// </summary>
        /// <param name="dto">The partner request to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created partner request.</returns>
        /// <response code="201">Partner request created.</response>
        /// <response code="400">Invalid request data.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerRequestDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Add([FromBody] PartnerRequestDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddPartnerRequest Name={Name}", dto.Name);

            var entity = dto.ToEntity();
            var result = await partnerRequestManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result.ToV2Dto());
        }

        /// <summary>
        /// Approves a partner request. Admin only.
        /// </summary>
        /// <param name="id">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The approved partner request.</returns>
        /// <response code="200">Partner request approved.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPut("{id}/approve")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 ApprovePartnerRequest Request={RequestId}", id);

            var result = await partnerRequestManager.ApproveBecomeAPartnerAsync(id, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Denies a partner request. Admin only.
        /// </summary>
        /// <param name="id">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The denied partner request.</returns>
        /// <response code="200">Partner request denied.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPut("{id}/deny")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(PartnerRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Deny(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DenyPartnerRequest Request={RequestId}", id);

            var result = await partnerRequestManager.DenyBecomeAPartnerAsync(id, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Gets all partner requests. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all partner requests.</returns>
        /// <response code="200">Returns the partner request list.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(typeof(IEnumerable<PartnerRequestDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetAllPartnerRequests");

            var results = await partnerRequestManager.GetAsync(cancellationToken);

            return Ok(results.Select(r => r.ToV2Dto()));
        }

        /// <summary>
        /// Gets a partner request by its unique identifier. Admin only.
        /// </summary>
        /// <param name="id">The partner request ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The partner request.</returns>
        /// <response code="200">Returns the partner request.</response>
        /// <response code="403">User is not an admin.</response>
        /// <response code="404">Partner request not found.</response>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [ProducesResponseType(typeof(PartnerRequestDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerRequest Request={RequestId}", id);

            var result = await partnerRequestManager.GetAsync(id, cancellationToken);

            if (result is null)
            {
                return NotFound();
            }

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Gets partner requests by user ID.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of partner requests for the user.</returns>
        /// <response code="200">Returns the partner request list.</response>
        [HttpGet("by-user/{userId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [ProducesResponseType(typeof(IEnumerable<PartnerRequestDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByUser(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetPartnerRequestsByUser User={UserId}", userId);

            var results = await partnerRequestManager.GetByCreatedUserIdAsync(userId, cancellationToken);

            return Ok(results.Select(r => r.ToV2Dto()));
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
