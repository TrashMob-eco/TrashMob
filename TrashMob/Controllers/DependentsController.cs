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
    /// Controller for managing dependents (minors linked to a user's account).
    /// </summary>
    [Route("api/users/{userId}/dependents")]
    public class DependentsController(
        IDependentManager dependentManager)
        : SecureController
    {
        /// <summary>
        /// Gets all active dependents for a user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>List of dependents.</returns>
        [HttpGet]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobReadScope)]
        [ProducesResponseType(typeof(IEnumerable<Dependent>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetDependents(Guid userId, CancellationToken cancellationToken)
        {
            if (userId != UserId)
            {
                return Forbid();
            }

            var dependents = await dependentManager.GetByParentUserIdAsync(userId, cancellationToken);
            return Ok(dependents);
        }

        /// <summary>
        /// Adds a new dependent to a user's account.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dependent">The dependent to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The created dependent.</returns>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Dependent), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddDependent(Guid userId, Dependent dependent, CancellationToken cancellationToken)
        {
            if (userId != UserId)
            {
                return Forbid();
            }

            dependent.ParentUserId = userId;
            var result = await dependentManager.AddAsync(dependent, UserId, cancellationToken);
            TrackEvent(nameof(AddDependent));

            return CreatedAtAction(nameof(GetDependents), new { userId }, result);
        }

        /// <summary>
        /// Updates an existing dependent.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="dependent">The updated dependent data.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>The updated dependent.</returns>
        [HttpPut("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(typeof(Dependent), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateDependent(
            Guid userId, Guid dependentId, Dependent dependent, CancellationToken cancellationToken)
        {
            if (userId != UserId)
            {
                return Forbid();
            }

            var existing = await dependentManager.GetAsync(dependentId, cancellationToken);
            if (existing == null || existing.ParentUserId != userId)
            {
                return NotFound();
            }

            dependent.Id = dependentId;
            dependent.ParentUserId = userId;
            var result = await dependentManager.UpdateAsync(dependent, UserId, cancellationToken);
            TrackEvent(nameof(UpdateDependent));

            return Ok(result);
        }

        /// <summary>
        /// Soft-deletes a dependent.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="dependentId">The dependent ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{dependentId}")]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        [RequiredScope(Constants.TrashMobWriteScope)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteDependent(
            Guid userId, Guid dependentId, CancellationToken cancellationToken)
        {
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
            TrackEvent(nameof(DeleteDependent));

            return NoContent();
        }
    }
}
