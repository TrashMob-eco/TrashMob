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
    /// V2 controller for managing dependents (minors linked to a user's account).
    /// </summary>
    [ApiController]
    [ApiVersion("2.0")]
    [EnableCors("_myAllowSpecificOrigins")]
    [Route("api/v{version:apiVersion}/users/{userId}/dependents")]
    public class DependentsV2Controller(
        IDependentManager dependentManager,
        ILogger<DependentsV2Controller> logger) : ControllerBase
    {
        private Guid UserId => Guid.TryParse(HttpContext.Items["UserId"]?.ToString(), out var parsedUserId) ? parsedUserId : Guid.Empty;

        /// <summary>
        /// Gets all active dependents for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the dependents.</response>
        /// <response code="403">Not the owner.</response>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IReadOnlyList<DependentDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDependents(Guid userId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 GetDependents for User={UserId}", userId);

            if (userId != UserId)
            {
                return Forbid();
            }

            var dependents = await dependentManager.GetByParentUserIdAsync(userId, cancellationToken);
            var dtos = dependents.Select(d => d.ToV2Dto()).ToList();

            return Ok(dtos);
        }

        /// <summary>
        /// Adds a new dependent to a user's account.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dto">The dependent data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="201">Dependent created.</response>
        /// <response code="403">Not the owner.</response>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> Add(Guid userId, DependentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 AddDependent for User={UserId}", userId);

            if (userId != UserId)
            {
                return Forbid();
            }

            var entity = dto.ToEntity();
            entity.ParentUserId = userId;

            var result = await dependentManager.AddAsync(entity, UserId, cancellationToken);

            return CreatedAtAction(nameof(GetDependents), new { userId }, result.ToV2Dto());
        }

        /// <summary>
        /// Updates an existing dependent.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="dto">The updated dependent data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="200">Returns the updated dependent.</response>
        /// <response code="403">Not the owner.</response>
        /// <response code="404">Dependent not found.</response>
        [HttpPut("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(DependentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(
            Guid userId, Guid dependentId, DependentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 UpdateDependent Id={DependentId} for User={UserId}", dependentId, userId);

            if (userId != UserId)
            {
                return Forbid();
            }

            var existing = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (existing == null || existing.ParentUserId != userId)
            {
                return NotFound();
            }

            var entity = dto.ToEntity();
            entity.Id = dependentId;
            entity.ParentUserId = userId;

            var result = await dependentManager.UpdateAsync(entity, UserId, cancellationToken);

            return Ok(result.ToV2Dto());
        }

        /// <summary>
        /// Soft-deletes a dependent.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <response code="204">Dependent deleted.</response>
        /// <response code="403">Not the owner.</response>
        /// <response code="404">Dependent not found.</response>
        [HttpDelete("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid userId, Guid dependentId, CancellationToken cancellationToken)
        {
            logger.LogInformation("V2 DeleteDependent Id={DependentId} for User={UserId}", dependentId, userId);

            if (userId != UserId)
            {
                return Forbid();
            }

            var existing = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (existing == null || existing.ParentUserId != userId)
            {
                return NotFound();
            }

            await dependentManager.SoftDeleteAsync(dependentId, UserId, cancellationToken);

            return NoContent();
        }
    }
}
