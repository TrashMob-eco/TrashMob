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
    public class LookupManager<T> : ILookupManager<T> where T : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LookupManager{T}"/> class.
        /// </summary>
        /// <param name="repository">The lookup repository for data access operations.</param>
        public LookupManager(ILookupRepository<T> repository)
        {
            Repository = repository;
        }

        /// <summary>
        /// Gets the repository used for data access operations.
        /// </summary>
        protected ILookupRepository<T> Repository { get; }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync()
        {
            return (await Repository.Get().ToListAsync()).AsEnumerable();
        }

        /// <inheritdoc />
        public virtual async Task<IEnumerable<T>> GetAsync(Expression<Func<T, bool>> expression)
        {
            return (await Repository.Get(expression).ToListAsync()).AsEnumerable();
        }

        /// <inheritdoc />
        public virtual Task<T> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return Repository.GetAsync(id, cancellationToken);
        }
    }
}