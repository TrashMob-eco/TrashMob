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
    /// V2 controller for admin management of community waiver assignments.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/admin/communities/{communityId}/waivers")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class CommunityWaiverAdminV2Controller(
        IWaiverVersionManager waiverVersionManager,
        ILogger<CommunityWaiverAdminV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all waiver assignments for a community.
        /// </summary>
        /// <param name="communityId">The community ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of community waiver assignments.</returns>
        /// <response code="200">Returns the community waiver assignments.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpGet]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<CommunityWaiverDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetCommunityWaivers(Guid communityId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetCommunityWaivers CommunityId={CommunityId}", communityId);

            var assignments = await waiverVersionManager.GetCommunityWaiverAssignmentsAsync(communityId, cancellationToken);

            return Ok(assignments.Select(a => a.ToV2Dto()));
        }

        /// <summary>
        /// Assigns a waiver to a community.
        /// </summary>
        /// <param name="communityId">The community ID.</param>
        /// <param name="request">The assignment request containing the waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created community waiver assignment.</returns>
        /// <response code="201">Waiver assigned to community.</response>
        /// <response code="400">Validation error.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(CommunityWaiverDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AssignWaiver(
            Guid communityId,
            [FromBody] AssignWaiverRequestDto request,
            CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AssignWaiverToCommunity CommunityId={CommunityId} WaiverId={WaiverId}", communityId, request.WaiverId);

            if (request.WaiverId == Guid.Empty)
            {
                return BadRequest("Waiver ID is required.");
            }

            var result = await waiverVersionManager.AssignToCommunityAsync(request.WaiverId, communityId, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetCommunityWaivers), new { communityId }, result.ToV2Dto());
        }

        /// <summary>
        /// Removes a waiver assignment from a community.
        /// </summary>
        /// <param name="communityId">The community ID.</param>
        /// <param name="waiverId">The waiver version ID to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Waiver removed from community.</response>
        /// <response code="403">User is not an admin.</response>
        [HttpDelete("{waiverId}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveWaiver(Guid communityId, Guid waiverId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 RemoveWaiverFromCommunity CommunityId={CommunityId} WaiverId={WaiverId}", communityId, waiverId);

            await waiverVersionManager.RemoveFromCommunityAsync(waiverId, communityId, cancellationToken);

            return NoContent();
        }
    }
}
