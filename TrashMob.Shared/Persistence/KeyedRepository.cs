namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Threading.Tasks;
    using System.Threading;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;

    /// <summary>
    /// Generic Implementation to save on boilerplate code
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class KeyedRepository<T> : BaseRepository<T>, IKeyedRepository<T> where T : KeyedModel
    {
        public KeyedRepository(MobDbContext mobDbContext) : base(mobDbContext)
        {
        }

        public override async Task<T> Add(T instance)
        {
            dbSet.Add(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        public override async Task<T> Update(T instance)
        {
            dbSet.Update(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        public async Task<T> Get(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<int> Delete(Guid id)
        {
            var instance = await dbSet.FindAsync(id).ConfigureAwait(false);
            dbSet.Remove(instance);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
