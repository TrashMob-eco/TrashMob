namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using System.Linq.Expressions;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;

    /// <summary>
    /// Generic Implementation to save on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LookupRepository<T> : ILookupRepository<T> where T : LookupModel
    {
        protected readonly MobDbContext mobDbContext;
        protected readonly DbSet<T> dbSet;

        public LookupRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
            dbSet = mobDbContext.Set<T>();

        }

        public IQueryable<T> Get()
        {
            return dbSet.AsNoTracking();
        }

        public IQueryable<T> Get(Expression<Func<T, bool>> expression)
        {
            return dbSet
                .Where(expression)
                .AsNoTracking();
        }

        public async Task<T> Get(int id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
