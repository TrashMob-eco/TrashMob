namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides data access implementation for entities derived from <see cref="BaseModel"/>.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from <see cref="BaseModel"/>.</typeparam>
    public class BaseRepository<T>(MobDbContext mobDbContext)
        : IBaseRepository<T> where T : BaseModel
    {
        protected readonly DbSet<T> dbSet = mobDbContext.Set<T>();
        protected readonly MobDbContext mobDbContext = mobDbContext;

        /// <inheritdoc />
        public virtual async Task<T> AddAsync(T instance)
        {
            dbSet.Add(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return instance;
        }

        /// <inheritdoc />
        public virtual async Task<T> UpdateAsync(T instance)
        {
            dbSet.Update(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return instance;
        }

        /// <inheritdoc />
        public IQueryable<T> Get()
        {
            return dbSet.AsNoTracking();
        }

        /// <inheritdoc />
        public IQueryable<T> Get(Expression<Func<T, bool>> expression, bool withNoTracking = true)
        {
            if (withNoTracking)
            {
                return dbSet
                    .Where(expression)
                    .AsNoTracking();
            }

            return dbSet
                .Where(expression);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(T instance)
        {
            dbSet.Remove(instance);
            var result = await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return result;
        }
    }
}