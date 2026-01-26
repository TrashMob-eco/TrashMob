namespace TrashMob.Controllers
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using TrashMob.Models;
    using TrashMob.Security;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Abstract controller for keyed entities, providing add, get, and delete operations.
    /// </summary>
    public abstract class KeyedController<T> : SecureController where T : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedController{T}"/> class.
        /// </summary>
        /// <param name="manager">The keyed manager.</param>
        public KeyedController(IKeyedManager<T> manager)
        {
            Manager = manager;
        }

        /// <summary>
        /// Gets the keyed manager.
        /// </summary>
        protected IKeyedManager<T> Manager { get; }

        /// <summary>
        /// Adds a new entity. Requires a valid user.
        /// </summary>
        /// <param name="instance">The entity to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Result of the add operation.</remarks>
        [HttpPost]
        [Authorize(Policy = AuthorizationPolicyConstants.ValidUser)]
        public virtual async Task<IActionResult> Add(T instance, CancellationToken cancellationToken)
        {
            await Manager.AddAsync(instance, UserId, cancellationToken).ConfigureAwait(false);

            TrackEvent("Add" + nameof(T));

            return Ok();
        }

        /// <summary>
        /// Gets all entities.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>List of entities.</remarks>
        [HttpGet]
        public virtual async Task<IActionResult> Get(CancellationToken cancellationToken)
        {
            var results = await Manager.GetAsync(cancellationToken).ConfigureAwait(false);

            TrackEvent("Get" + nameof(T));

            return Ok(results);
        }

        /// <summary>
        /// Deletes an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <remarks>Result of the delete operation.</remarks>
        [HttpDelete("{id}")]
        public virtual async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
        {
            var entity = Manager.GetAsync(id, cancellationToken);

            var authResult =
                await AuthorizationService.AuthorizeAsync(User, entity, AuthorizationPolicyConstants.UserOwnsEntity);

            if (!User.Identity.IsAuthenticated || !authResult.Succeeded)
            {
                return Forbid();
            }

            var results = await Manager.DeleteAsync(id, cancellationToken).ConfigureAwait(false);

            TrackEvent("Delete" + nameof(T));

            return Ok(results);
        }
    }
}