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
    using TrashMob.Models;
    using TrashMob.Models.Extensions.V2;
    using TrashMob.Models.Poco.V2;
    using TrashMob.Security;
    using TrashMob.Shared;
    using TrashMob.Shared.Managers.Contacts;

    /// <summary>
    /// V2 admin controller for grant task management.
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/granttasks")]
    [Authorize(Policy = AuthorizationPolicyConstants.UserIsAdmin)]
    public class GrantTasksV2Controller(
        IGrantTaskManager grantTaskManager,
        ILogger<GrantTasksV2Controller> logger) : ControllerBase
    {
        private Guid UserId => new(HttpContext.Items["UserId"]?.ToString() ?? string.Empty);

        /// <summary>
        /// Gets all tasks for a specific grant.
        /// </summary>
        /// <param name="grantId">The grant identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A list of grant tasks.</returns>
        /// <response code="200">Returns grant tasks.</response>
        [HttpGet("bygrant/{grantId}")]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<GrantTaskDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByGrantId(Guid grantId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetByGrantId GrantTasks Grant={GrantId} by User={UserId}", grantId, UserId);

            var tasks = await grantTaskManager.GetByGrantIdAsync(grantId, cancellationToken);

            return Ok(tasks.Select(t => t.ToV2Dto()));
        }

        /// <summary>
        /// Creates a new grant task.
        /// </summary>
        /// <param name="taskDto">The grant task to create.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created grant task.</returns>
        /// <response code="201">Grant task created.</response>
        [HttpPost]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(GrantTaskDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> Create(GrantTaskDto taskDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Create GrantTask for Grant={GrantId} by User={UserId}", taskDto.GrantId, UserId);

            var entity = taskDto.ToEntity();
            entity.CreatedByUserId = UserId;
            entity.CreatedDate = DateTimeOffset.UtcNow;
            entity.LastUpdatedByUserId = UserId;
            entity.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await grantTaskManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetByGrantId), new { grantId = result.GrantId }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing grant task.
        /// </summary>
        /// <param name="taskDto">The grant task to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated grant task.</returns>
        /// <response code="200">Grant task updated.</response>
        [HttpPut]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(GrantTaskDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(GrantTaskDto taskDto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Update GrantTask={TaskId} by User={UserId}", taskDto.Id, UserId);

            var entity = taskDto.ToEntity();
            entity.LastUpdatedByUserId = UserId;
            entity.LastUpdatedDate = DateTimeOffset.UtcNow;

            var result = await grantTaskManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Deletes a grant task.
        /// </summary>
        /// <param name="id">The grant task identifier.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        /// <response code="204">Grant task deleted.</response>
        [HttpDelete("{id}")]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 Delete GrantTask={TaskId} by User={UserId}", id, UserId);

            await grantTaskManager.DeleteAsync(id, cancellationToken);

            return NoContent();
        }
    }
}
