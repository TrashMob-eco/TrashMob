namespace TrashMob.Shared.Persistence
{
    using System.Threading.Tasks;
    using TrashMob.Shared.Persistence.Interfaces;

    public class DbTransaction : IDbTransaction
    {
        protected readonly MobDbContext mobDbContext;

        public DbTransaction(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task BeginTransactionAsync()
        {
            await mobDbContext.Database.BeginTransactionAsync();
        }

        public async Task CommitTransactionAsync()
        {
            await mobDbContext.Database.CommitTransactionAsync();
        }

        public async Task RollbackTransactionAsync()
        {
            await mobDbContext.Database.RollbackTransactionAsync();
        }
    }
}