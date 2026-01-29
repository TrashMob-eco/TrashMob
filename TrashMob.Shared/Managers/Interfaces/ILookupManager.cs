namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing lookup/reference data entities with integer keys.
    /// </summary>
    /// <typeparam name="T">The lookup entity type derived from LookupModel.</typeparam>
    public interface ILookupManager<T> where T : LookupModel
    {
        /// <summary>
        /// Retrieves all lookup values.
        /// </summary>
        /// <returns>A collection of all lookup entities.</returns>
        Task<IEnumerable<T>> GetAsync();

        /// <summary>
        /// Retrieves lookup values matching the specified expression.
        /// </summary>
        /// <param name="expression">The filter expression.</param>
        /// <returns>A collection of matching lookup entities.</returns>
        Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression);

        /// <summary>
        /// Retrieves a lookup value by its integer ID.
        /// </summary>
        /// <param name="id">The lookup ID.</param>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns>The lookup entity or null if not found.</returns>
        Task<T> GetAsync(int id, CancellationToken cancellationToken = default);
    }
}