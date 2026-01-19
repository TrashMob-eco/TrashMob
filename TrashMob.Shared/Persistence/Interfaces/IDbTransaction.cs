namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Threading.Tasks;

    /// <summary>
    /// Defines database transaction operations for managing transactional data access.
    /// </summary>
    public interface IDbTransaction
    {
        /// <summary>
        /// Begins a new database transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task BeginTransactionAsync();

        /// <summary>
        /// Commits the current database transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task CommitTransactionAsync();

        /// <summary>
        /// Rolls back the current database transaction.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation.</returns>
        Task RollbackTransactionAsync();
    }
}