namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines data access operations for entities derived from <see cref="KeyedModel"/> that have a <see cref="Guid"/> identifier.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from <see cref="KeyedModel"/>.</typeparam>
    public interface IKeyedRepository<T> : IBaseRepository<T> where T : KeyedModel
    {
        /// <summary>
        /// Deletes an entity from the repository by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> DeleteAsync(Guid id);

        /// <summary>
        /// Gets an entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken);

        /// <summary>
        /// Gets an entity by its identifier without change tracking.
        /// </summary>
        /// <param name="id">The unique identifier of the entity to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the entity if found; otherwise, null.</returns>
        Task<T> GetWithNoTrackingAsync(Guid id, CancellationToken cancellationToken);
    }
}