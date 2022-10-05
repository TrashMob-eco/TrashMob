namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Linq.Expressions;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;
    using System.Threading;

    /// <summary>
    /// Generic Implementation to save on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BaseRepository<T> : IBaseRepository<T> where T : BaseModel
    {
        protected readonly MobDbContext mobDbContext;
        protected readonly DbSet<T> dbSet;

        public BaseRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
            dbSet = mobDbContext.Set<T>();

        }

        public virtual async Task<T> AddAsync(T instance, CancellationToken cancellationToken = default)
        {
            dbSet.Add(instance);
            await mobDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return instance;
        }

        public virtual async Task<T> UpdateAsync(T instance, CancellationToken cancellationToken = default)
        {
            dbSet.Update(instance);
            await mobDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return instance;
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

        public async Task<int> DeleteAsync(T instance, CancellationToken cancellationToken = default)
        {
            dbSet.Remove(instance);
            var result = await mobDbContext.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
            return result;
        }
    }
}
