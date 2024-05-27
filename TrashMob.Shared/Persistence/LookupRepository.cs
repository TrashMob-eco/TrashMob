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
    ///     Generic Implementation to save on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LookupRepository<T> : ILookupRepository<T> where T : LookupModel
    {
        protected readonly DbSet<T> dbSet;
        protected readonly MobDbContext mobDbContext;

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

        public async Task<T> GetAsync(int id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}