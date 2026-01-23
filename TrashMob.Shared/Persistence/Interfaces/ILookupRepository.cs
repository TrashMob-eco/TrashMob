namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines read-only data access operations for lookup entities derived from <see cref="LookupModel"/>.
    /// </summary>
    /// <typeparam name="T">The lookup entity type that derives from <see cref="LookupModel"/>.</typeparam>
    public interface ILookupRepository<T> where T : LookupModel
    {
        /// <summary>
        /// Gets all lookup entities from the repository.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> that can be used to query lookup entities.</returns>
        IQueryable<T> Get();

        /// <summary>
        /// Gets lookup entities from the repository that match the specified filter expression.
        /// </summary>
        /// <param name="expression">A filter expression to apply to the query.</param>
        /// <returns>An <see cref="IQueryable{T}"/> containing lookup entities that match the filter.</returns>
        IQueryable<T> Get(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Gets a lookup entity by its identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the lookup entity to retrieve.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the lookup entity if found; otherwise, null.</returns>
        Task<T> GetAsync(int id, CancellationToken cancellationToken);
    }
}