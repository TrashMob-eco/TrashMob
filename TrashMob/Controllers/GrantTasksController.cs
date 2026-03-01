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
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// Controller for grant task management (admin only).
    /// </summary>
    [Route("api/granttasks")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    [RequiredScope(Constants.TrashMobWriteScope)]
    public class GrantTasksController(IGrantTaskManager grantTaskManager)
        : SecureController
    {
        /// <summary>
        /// Gets all tasks for a specific grant.
        /// </summary>
        /// <param name="grantId">The grant ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpGet("bygrant/{grantId}")]
        [ProducesResponseType(typeof(IEnumerable<GrantTask>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByGrantId(Guid grantId, CancellationToken cancellationToken)
        {
            var tasks = await grantTaskManager.GetByGrantIdAsync(grantId, cancellationToken);
            return Ok(tasks);
        }

        /// <summary>
        /// Creates a new grant task.
        /// </summary>
        /// <param name="grantTask">The task to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPost]
        [ProducesResponseType(typeof(GrantTask), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(GrantTask grantTask, CancellationToken cancellationToken)
        {
            var result = await grantTaskManager.AddAsync(grantTask, UserId, cancellationToken);
            TrackEvent("AddGrantTask");
            return CreatedAtAction(nameof(GetByGrantId), new { grantId = result.GrantId }, result);
        }

        /// <summary>
        /// Updates a grant task.
        /// </summary>
        /// <param name="grantTask">The updated task data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpPut]
        [ProducesResponseType(typeof(GrantTask), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(GrantTask grantTask, CancellationToken cancellationToken)
        {
            var result = await grantTaskManager.UpdateAsync(grantTask, UserId, cancellationToken);
            TrackEvent("UpdateGrantTask");
            return Ok(result);
        }

        /// <summary>
        /// Deletes a grant task.
        /// </summary>
        /// <param name="id">The task ID to delete.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            await grantTaskManager.DeleteAsync(id, cancellationToken);
            TrackEvent("DeleteGrantTask");
            return NoContent();
        }
    }
}
