namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing event summaries and statistics.
    /// </summary>
    public interface IEventSummaryManager : IBaseManager<EventSummary>
    {
        /// <summary>
        /// Gets aggregate statistics for all events.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The aggregate statistics.</returns>
        Task<Stats> GetStatsAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Gets aggregate statistics for events associated with a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The aggregate statistics for the specified user.</returns>
        Task<Stats> GetStatsByUser(Guid userId, CancellationToken cancellationToken);

        /// <summary>
        /// Gets event summaries filtered by location criteria.
        /// </summary>
        /// <param name="locationFilter">The location filter criteria.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of display event summaries matching the filter.</returns>
        Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter,
            CancellationToken cancellationToken);
    }
}
