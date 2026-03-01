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
    using TrashMob.Shared.Managers.Contacts;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Controller for grant management (admin only).
    /// </summary>
    [Route("api/grants")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class GrantsController(
        IGrantManager grantManager,
        IGrantDiscoveryService grantDiscoveryService)
        : SecureController
    {
        /// <summary>
        /// Gets all grants, optionally filtered by status.
        /// </summary>
        /// <param name="status">Optional grant status filter.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Grant>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll([FromQuery] int? status, CancellationToken cancellationToken)
        {
            IEnumerable<Grant> grants;

            if (status.HasValue)
            {
                grants = await grantManager.GetByStatusAsync(status.Value, cancellationToken);
            }
            else
            {
                grants = await grantManager.GetAsync(cancellationToken);
            }

            return Ok(grants);
        }

        /// <summary>
        /// Gets a grant by ID.
        /// </summary>
        /// <param name="id">The grant ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Grant), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var grant = await grantManager.GetAsync(id, cancellationToken);

            if (grant is null)
            {
                return NotFound();
            }

            return Ok(grant);
        }

        /// <summary>
        /// Creates a new grant.
        /// </summary>
        /// <param name="grant">The grant to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(Grant), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(Grant grant, CancellationToken cancellationToken)
        {
            var result = await grantManager.AddAsync(grant, UserId, cancellationToken);
            TrackEvent("AddGrant");
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
        }

        /// <summary>
        /// Updates a grant.
        /// </summary>
        /// <param name="grant">The updated grant data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(Grant), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Grant grant, CancellationToken cancellationToken)
        {
            var result = await grantManager.UpdateAsync(grant, UserId, cancellationToken);
            TrackEvent("UpdateGrant");
            return Ok(result);
        }

        /// <summary>
        /// Discovers new grant opportunities using AI.
        /// </summary>
        /// <param name="request">The discovery request with optional prompt or focus areas.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("discover")]
        [ProducesResponseType(typeof(GrantDiscoveryResult), StatusCodes.Status200OK)]
        public async Task<IActionResult> Discover(GrantDiscoveryRequest request, CancellationToken cancellationToken)
        {
            var result = await grantDiscoveryService.DiscoverGrantsAsync(request, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a grant.
        /// </summary>
        /// <param name="id">The grant ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await grantManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteGrant");
            return NoContent();
        }
    }
}
