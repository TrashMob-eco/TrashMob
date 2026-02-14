namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides database transaction implementation for managing transactional data access.
    /// </summary>
    public class DbTransaction(MobDbContext mobDbContext) : IDbTransaction
    {

        /// <inheritdoc />
        public async Task BeginTransactionAsync()
        {
            await mobDbContext.Database.BeginTransactionAsync();
        }

        /// <inheritdoc />
        public async Task CommitTransactionAsync()
        {
            await mobDbContext.Database.CommitTransactionAsync();
        }

        /// <inheritdoc />
        public async Task RollbackTransactionAsync()
        {
            await mobDbContext.Database.RollbackTransactionAsync();
        }
    }
}