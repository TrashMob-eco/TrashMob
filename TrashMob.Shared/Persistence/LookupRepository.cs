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
    public class LookupRepository<T>(MobDbContext mobDbContext)
        : ILookupRepository<T> where T : LookupModel
    {
        protected readonly DbSet<T> dbSet = mobDbContext.Set<T>();

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