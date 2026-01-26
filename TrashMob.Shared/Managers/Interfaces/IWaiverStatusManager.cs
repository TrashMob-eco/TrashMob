namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing waivers.
    /// </summary>
    public interface IWaiverManager : IKeyedManager<Waiver>
    {
        /// <summary>
        /// Gets a waiver by its name.
        /// </summary>
        /// <param name="waiverName">The name of the waiver.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The waiver with the specified name, or null if not found.</returns>
        Task<Waiver> GetByNameAsync(string waiverName, CancellationToken cancellationToken = default);
    }
}
