namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines base CRUD operations for entity managers.
    /// </summary>
    /// <typeparam name="T">The entity type derived from BaseModel.</typeparam>
    public interface IBaseManager<T> where T : BaseModel
    {
        /// <summary>
        /// Adds a new entity instance.
        /// </summary>
        /// <param name="instance">The entity to add.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The added entity.</returns>
        Task<T> AddAsync(T instance, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing entity instance.
        /// </summary>
        /// <param name="instance">The entity to update.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T instance, CancellationToken cancellationToken);

        /// <summary>
        /// Adds a new entity instance with user tracking.
        /// </summary>
        /// <param name="instance">The entity to add.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The added entity.</returns>
        Task<T> AddAsync(T instance, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Updates an existing entity instance with user tracking.
        /// </summary>
        /// <param name="instance">The entity to update.</param>
        /// <param name="userId">The ID of the user performing the action.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The updated entity.</returns>
        Task<T> UpdateAsync(T instance, Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves all entities.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of all entities.</returns>
        Task<IEnumerable<T>> GetAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves entities matching the specified expression.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of matching entities.</returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves entities created by the specified user.
        /// </summary>
        /// <param name="userId">The user ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of entities created by the user.</returns>
        Task<IEnumerable<T>> GetByCreatedUserIdAsync(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves entities by their parent ID.
        /// </summary>
        /// <param name="parentId">The parent entity ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of child entities.</returns>
        Task<IEnumerable<T>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an entity by composite key (GUID and integer).
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="secondId">The secondary integer ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The matching entity or null.</returns>
        Task<T> GetAsync(Guid parentId, int secondId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves an entity by composite key (two GUIDs).
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="secondId">The secondary GUID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The matching entity or null.</returns>
        Task<T> GetAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a collection of entities by composite key.
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="secondId">The secondary GUID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>A collection of matching entities.</returns>
        Task<IEnumerable<T>> GetCollectionAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity by its primary ID.
        /// </summary>
        /// <param name="parentId">The entity ID to delete.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity by composite key (GUID and integer).
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="secondId">The secondary integer ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> Delete(Guid parentId, int secondId, CancellationToken cancellationToken);

        /// <summary>
        /// Deletes an entity by composite key (two GUIDs).
        /// </summary>
        /// <param name="parentId">The parent ID.</param>
        /// <param name="secondId">The secondary GUID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The number of affected records.</returns>
        Task<int> Delete(Guid parentId, Guid secondId, CancellationToken cancellationToken);
    }
}