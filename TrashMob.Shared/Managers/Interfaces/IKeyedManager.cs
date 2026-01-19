namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing entities with a primary GUID key.
    /// </summary>
    /// <typeparam name="T">The entity type derived from KeyedModel.</typeparam>
    public interface IKeyedManager<T> : IBaseManager<T> where T : KeyedModel
    {
        /// <summary>
        /// Retrieves an entity by its unique identifier.
        /// </summary>
        /// <param name="id">The entity ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The entity or null if not found.</returns>
        Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default);
    }
}