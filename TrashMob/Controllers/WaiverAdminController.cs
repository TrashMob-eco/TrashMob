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
    /// Controller for waiver administration operations.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/waivers")]
    public class WaiverAdminController : SecureController
    {
        private readonly IWaiverVersionManager waiverVersionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="WaiverAdminController"/> class.
        /// </summary>
        /// <param name="waiverVersionManager">The waiver version manager.</param>
        public WaiverAdminController(IWaiverVersionManager waiverVersionManager)
        {
            this.waiverVersionManager = waiverVersionManager;
        }

        /// <summary>
        /// Gets all waiver versions. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of all waiver versions.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(CancellationToken cancellationToken = default)
        {
            var result = await waiverVersionManager.GetAllAsync(cancellationToken);
            TrackEvent(nameof(GetAll));

            return Ok(result);
        }

        /// <summary>
        /// Gets active waiver versions. Admin only.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of active waiver versions.</returns>
        [HttpGet("active")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<WaiverVersion>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActive(CancellationToken cancellationToken = default)
        {
            var result = await waiverVersionManager.GetActiveWaiversAsync(cancellationToken);
            TrackEvent(nameof(GetActive));

            return Ok(result);
        }

        /// <summary>
        /// Gets a waiver version by ID. Admin only.
        /// </summary>
        /// <param name="id">Waiver version ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The waiver version or 404 if not found.</returns>
        [HttpGet("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(WaiverVersion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken = default)
        {
            var result = await waiverVersionManager.GetAsync(id, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            TrackEvent(nameof(Get));

            return Ok(result);
        }

        /// <summary>
        /// Creates a new waiver version. Admin only.
        /// </summary>
        /// <param name="waiverVersion">The waiver version to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created waiver version.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(WaiverVersion), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Create(
            [FromBody] WaiverVersion waiverVersion,
            CancellationToken cancellationToken = default)
        {
            if (waiverVersion == null)
            {
                return BadRequest("Waiver version is required.");
            }

            if (string.IsNullOrWhiteSpace(waiverVersion.Name))
            {
                return BadRequest("Waiver name is required.");
            }

            if (string.IsNullOrWhiteSpace(waiverVersion.Version))
            {
                return BadRequest("Waiver version string is required.");
            }

            if (string.IsNullOrWhiteSpace(waiverVersion.WaiverText))
            {
                return BadRequest("Waiver text is required.");
            }

            var result = await waiverVersionManager.AddAsync(waiverVersion, UserId, cancellationToken);
            TrackEvent(nameof(Create));

            return CreatedAtAction(nameof(Get), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates an existing waiver version with new values. Admin only.
        /// </summary>
        /// <param name="id">The unique identifier of the waiver version to update.</param>
        /// <param name="waiverVersion">The waiver version object with updated values.</param>
        /// <param name="cancellationToken">Token to cancel the operation.</param>
        /// <returns>The modified waiver version.</returns>
        [HttpPut("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(WaiverVersion), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid id,
            [FromBody] WaiverVersion waiverVersion,
            CancellationToken cancellationToken = default)
        {
            if (waiverVersion == null || waiverVersion.Id != id)
            {
                return BadRequest("Waiver version ID mismatch.");
            }

            var existing = await waiverVersionManager.GetAsync(id, cancellationToken);
            if (existing == null)
            {
                return NotFound();
            }

            waiverVersion.LastUpdatedByUserId = UserId;
            waiverVersion.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await waiverVersionManager.UpdateAsync(waiverVersion, cancellationToken);
            TrackEvent(nameof(Update));

            return Ok(result);
        }

        /// <summary>
        /// Deactivates a waiver version. Admin only.
        /// </summary>
        /// <param name="id">Waiver version ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{id}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Deactivate(Guid id, CancellationToken cancellationToken = default)
        {
            try
            {
                await waiverVersionManager.DeactivateAsync(id, UserId, cancellationToken);
                TrackEvent(nameof(Deactivate));

                return NoContent();
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
        }
    }

    /// <summary>
    /// Controller for community waiver assignment operations.
    /// All endpoints require site admin privileges.
    /// </summary>
    [Route("api/admin/communities/{communityId}/waivers")]
    public class CommunityWaiverAdminController : SecureController
    {
        private readonly IWaiverVersionManager waiverVersionManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityWaiverAdminController"/> class
        /// for managing waiver assignments to communities.
        /// </summary>
        /// <param name="waiverVersionManager">The manager for waiver version operations.</param>
        public CommunityWaiverAdminController(IWaiverVersionManager waiverVersionManager)
        {
            this.waiverVersionManager = waiverVersionManager;
        }

        /// <summary>
        /// Gets all waiver assignments for a community. Admin only.
        /// </summary>
        /// <param name="communityId">Community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of community waiver assignments.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<CommunityWaiver>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCommunityWaivers(
            Guid communityId,
            CancellationToken cancellationToken = default)
        {
            var result = await waiverVersionManager.GetCommunityWaiverAssignmentsAsync(communityId, cancellationToken);
            TrackEvent(nameof(GetCommunityWaivers));

            return Ok(result);
        }

        /// <summary>
        /// Assigns a waiver to a community. Admin only.
        /// </summary>
        /// <param name="communityId">Community (partner) ID.</param>
        /// <param name="request">Assignment request containing waiver ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created community waiver assignment.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(CommunityWaiver), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AssignWaiver(
            Guid communityId,
            [FromBody] AssignWaiverRequest request,
            CancellationToken cancellationToken = default)
        {
            if (request == null || request.WaiverId == Guid.Empty)
            {
                return BadRequest("Waiver ID is required.");
            }

            try
            {
                var result = await waiverVersionManager.AssignToCommunityAsync(
                    request.WaiverId,
                    communityId,
                    UserId,
                    cancellationToken);
                TrackEvent(nameof(AssignWaiver));

                return CreatedAtAction(nameof(GetCommunityWaivers), new { communityId }, result);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// Removes a waiver assignment from a community. Admin only.
        /// </summary>
        /// <param name="communityId">Community (partner) ID.</param>
        /// <param name="waiverId">Waiver version ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{waiverId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> RemoveWaiver(
            Guid communityId,
            Guid waiverId,
            CancellationToken cancellationToken = default)
        {
            await waiverVersionManager.RemoveFromCommunityAsync(waiverId, communityId, cancellationToken);
            TrackEvent(nameof(RemoveWaiver));

            return NoContent();
        }
    }

    /// <summary>
    /// Request model for assigning a waiver to a community.
    /// </summary>
    public class AssignWaiverRequest
    {
        /// <summary>
        /// Gets or sets the waiver version ID to assign.
        /// </summary>
        public Guid WaiverId { get; set; }
    }
}
