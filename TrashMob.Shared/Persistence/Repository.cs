namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using System.Threading;
    using System.Linq.Expressions;

    /// <summary>
    /// Generic Implementation to save on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Repository<T> : IRepository<T> where T : BaseModel
    {
        protected readonly MobDbContext mobDbContext;
        protected readonly DbSet<T> dbSet;

        public Repository(MobDbContext mobDbContext)
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

        public async Task<T> Add(T instance)
        {
            dbSet.Add(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        public async Task<T> Update(T instance)
        {
            dbSet.Update(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> Delete(Guid id)
        {
            var instance = await dbSet.FindAsync(id).ConfigureAwait(false);
            dbSet.Remove(instance);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
