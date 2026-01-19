namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines data access operations for entities derived from <see cref="BaseModel"/>.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from <see cref="BaseModel"/>.</typeparam>
    public interface IBaseRepository<T> where T : BaseModel
    {
        /// <summary>
        /// Adds a new entity to the repository.
        /// </summary>
        /// <param name="instance">The entity instance to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the added entity.</returns>
        Task<T> AddAsync(T instance);

        /// <summary>
        /// Updates an existing entity in the repository.
        /// </summary>
        /// <param name="instance">The entity instance with updated values.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the updated entity.</returns>
        Task<T> UpdateAsync(T instance);

        /// <summary>
        /// Gets all entities from the repository.
        /// </summary>
        /// <returns>An <see cref="IQueryable{T}"/> that can be used to query entities.</returns>
        IQueryable<T> Get();

        /// <summary>
        /// Gets entities from the repository that match the specified filter expression.
        /// </summary>
        /// <param name="expression">A filter expression to apply to the query.</param>
        /// <param name="withNoTracking">If true, entities are retrieved without change tracking; otherwise, change tracking is enabled.</param>
        /// <returns>An <see cref="IQueryable{T}"/> containing entities that match the filter.</returns>
        IQueryable<T> Get(Expression<Func<T, bool>> expression, bool withNoTracking = true);

        /// <summary>
        /// Deletes an entity from the repository.
        /// </summary>
        /// <param name="instance">The entity instance to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of state entries written to the database.</returns>
        Task<int> DeleteAsync(T instance);
    }
}