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
    using TrashMob.Shared.Managers.Prospects;

    /// <summary>
    /// Controller for community prospect management (admin only).
    /// </summary>
    [Route("api/communityprospects")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class CommunityProspectsController(
        ICommunityProspectManager communityProspectManager,
        IProspectActivityManager prospectActivityManager)
        : SecureController
    {
        /// <summary>
        /// Gets all community prospects, optionally filtered by pipeline stage or search term.
        /// </summary>
        /// <param name="stage">Optional pipeline stage filter.</param>
        /// <param name="search">Optional search term.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<CommunityProspect>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAll(
            [FromQuery] int? stage,
            [FromQuery] string search,
            CancellationToken cancellationToken)
        {
            IEnumerable<CommunityProspect> prospects;

            if (!string.IsNullOrWhiteSpace(search))
            {
                prospects = await communityProspectManager.SearchAsync(search, cancellationToken);
            }
            else if (stage.HasValue)
            {
                prospects = await communityProspectManager.GetByPipelineStageAsync(stage.Value, cancellationToken);
            }
            else
            {
                prospects = await communityProspectManager.GetAsync(cancellationToken);
            }

            return Ok(prospects);
        }

        /// <summary>
        /// Gets a community prospect by ID.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
        {
            var prospect = await communityProspectManager.GetAsync(id, cancellationToken);

            if (prospect == null)
            {
                return NotFound();
            }

            return Ok(prospect);
        }

        /// <summary>
        /// Creates a new community prospect.
        /// </summary>
        /// <param name="prospect">The prospect to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        public async Task<IActionResult> Create(CommunityProspect prospect, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.AddAsync(prospect, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Updates a community prospect.
        /// </summary>
        /// <param name="prospect">The updated prospect data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(CommunityProspect prospect, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.UpdateAsync(prospect, UserId, cancellationToken);
            return Ok(result);
        }

        /// <summary>
        /// Deletes a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await communityProspectManager.DeleteAsync(id, cancellationToken);
            return Ok();
        }

        /// <summary>
        /// Updates the pipeline stage of a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="request">The new pipeline stage.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut("{id}/stage")]
        [ProducesResponseType(typeof(CommunityProspect), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStage(Guid id, [FromBody] UpdateStageRequest request, CancellationToken cancellationToken)
        {
            var result = await communityProspectManager.UpdatePipelineStageAsync(id, request.Stage, UserId, cancellationToken);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Gets all activities for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("{id}/activities")]
        [ProducesResponseType(typeof(IEnumerable<ProspectActivity>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetActivities(Guid id, CancellationToken cancellationToken)
        {
            var activities = await prospectActivityManager.GetByProspectIdAsync(id, cancellationToken);
            return Ok(activities);
        }

        /// <summary>
        /// Creates a new activity for a community prospect.
        /// </summary>
        /// <param name="id">The prospect ID.</param>
        /// <param name="activity">The activity to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost("{id}/activities")]
        [ProducesResponseType(typeof(ProspectActivity), StatusCodes.Status200OK)]
        public async Task<IActionResult> CreateActivity(Guid id, ProspectActivity activity, CancellationToken cancellationToken)
        {
            activity.ProspectId = id;
            var result = await prospectActivityManager.AddAsync(activity, UserId, cancellationToken);
            return Ok(result);
        }
    }

    /// <summary>
    /// Request body for updating a prospect's pipeline stage.
    /// </summary>
    public class UpdateStageRequest
    {
        /// <summary>
        /// Gets or sets the new pipeline stage value.
        /// </summary>
        public int Stage { get; set; }
    }
}
