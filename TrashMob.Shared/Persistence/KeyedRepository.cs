namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides data access implementation for entities derived from <see cref="KeyedModel"/> that have a <see cref="Guid"/> identifier.
    /// </summary>
    /// <typeparam name="T">The entity type that derives from <see cref="KeyedModel"/>.</typeparam>
    public class KeyedRepository<T> : BaseRepository<T>, IKeyedRepository<T> where T : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="KeyedRepository{T}"/> class.
        /// </summary>
        /// <param name="mobDbContext">The database context to use for data access.</param>
        public KeyedRepository(MobDbContext mobDbContext) : base(mobDbContext)
        {
        }

        /// <inheritdoc />
        public override async Task<T> AddAsync(T instance)
        {
            dbSet.Add(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public override async Task<T> UpdateAsync(T instance)
        {
            dbSet.Update(instance);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await dbSet.FindAsync(instance.Id).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet.FindAsync(new object[] { id }, cancellationToken).ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<T> GetWithNoTrackingAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await dbSet.AsNoTracking().FirstOrDefaultAsync(e => e.Id == id, cancellationToken)
                .ConfigureAwait(false);
        }

        /// <inheritdoc />
        public async Task<int> DeleteAsync(Guid id)
        {
            var instance = await dbSet.FindAsync(id).ConfigureAwait(false);
            dbSet.Remove(instance);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}