namespace TrashMob.Shared.Persistence.Interfaces
{    
    using System.Threading.Tasks;

    public interface IDbTransaction
    {
        Task BeginTransactionAsync();
        Task CommitTransactionAsync();
        Task RollbackTransactionAsync();
    }
}
