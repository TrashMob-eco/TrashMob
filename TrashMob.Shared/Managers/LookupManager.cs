namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager class for lookup/reference data entities with integer primary keys.
    /// </summary>
    /// <typeparam name="T">The entity type derived from LookupModel.</typeparam>
    public class LookupManager<T>(ILookupRepository<T> repository)
        : ILookupManager<T> where T : LookupModel
    {
        /// <summary>
        /// Gets the repository used for data access operations.
        /// </summary>
        protected ILookupRepository<T> Repository { get; } = repository;

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync()
        {
            return await Repository.Get().ToListAsync();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression)
        {
            return await Repository.Get(expression).ToListAsync();
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return Repository.GetAsync(id, cancellationToken);
        }
    }
}