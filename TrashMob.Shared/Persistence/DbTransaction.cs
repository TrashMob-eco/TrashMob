namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Provides database transaction implementation for managing transactional data access.
    /// </summary>
    public class DbTransaction : IDbTransaction
    {
        /// <summary>
        /// The database context.
        /// </summary>
        protected readonly MobDbContext mobDbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbTransaction"/> class.
        /// </summary>
        /// <param name="mobDbContext">The database context to use for transaction management.</param>
        public DbTransaction(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

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