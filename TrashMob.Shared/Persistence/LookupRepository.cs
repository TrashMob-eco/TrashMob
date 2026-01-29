namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides read-only data access implementation for lookup entities derived from <see cref="LookupModel"/>.
    /// </summary>
    /// <typeparam name="T">The lookup entity type that derives from <see cref="LookupModel"/>.</typeparam>
    public class LookupRepository<T> : ILookupRepository<T> where T : LookupModel
    {
        /// <summary>
        /// The database set for the lookup entity type.
        /// </summary>
        protected readonly DbSet<T> dbSet;

        /// <summary>
        /// The database context.
        /// </summary>
        protected readonly MobDbContext mobDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LookupRepository{T}"/> class.
        /// </summary>
        /// <param name="mobDbContext">The database context to use for data access.</param>
        public LookupRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
            dbSet = mobDbContext.Set<T>();
        }

        /// <inheritdoc />
        public IQueryable<T> Get()
        {
            return dbSet.AsNoTracking();
        }

        /// <inheritdoc />
        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return dbSet
                .Where(expression)
                .AsNoTracking();
        }

        /// <inheritdoc />
        public async Task<T> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}