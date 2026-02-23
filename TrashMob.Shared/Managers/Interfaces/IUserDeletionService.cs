namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Handles comprehensive user data deletion and anonymization.
    /// Ensures all user PII is removed or anonymized across every table,
    /// wrapped in a transaction for atomicity.
    /// </summary>
    public interface IUserDeletionService
    {
        /// <summary>
        /// Deletes or anonymizes all user data across every table that references the user.
        /// Operations are wrapped in a database transaction for atomicity.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to delete.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        Task<int> DeleteUserDataAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
