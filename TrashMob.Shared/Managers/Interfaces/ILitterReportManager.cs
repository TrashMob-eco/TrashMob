namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Defines operations for managing litter reports.
    /// </summary>
    public interface ILitterReportManager : IKeyedManager<LitterReport>
    {
        /// <summary>
        /// Gets all litter reports with a new status.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of new litter reports.</returns>
        Task<IEnumerable<LitterReport>> GetNewLitterReportsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all litter reports that have been assigned.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of assigned litter reports.</returns>
        Task<IEnumerable<LitterReport>> GetAssignedLitterReportsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all litter reports that have been cleaned.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of cleaned litter reports.</returns>
        Task<IEnumerable<LitterReport>> GetCleanedLitterReportsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all litter reports that have not been cancelled.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of non-cancelled litter reports.</returns>
        Task<IEnumerable<LitterReport>>
            GetNotCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all litter reports that have been cancelled.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of cancelled litter reports.</returns>
        Task<IEnumerable<LitterReport>> GetCancelledLitterReportsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all litter reports created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of litter reports created by the user.</returns>
        Task<IEnumerable<LitterReport>> GetUserLitterReportsAsync(Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets litter reports matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria for litter reports.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of litter reports matching the filter.</returns>
        public Task<IEnumerable<LitterReport>> GetFilteredLitterReportsAsync(LitterReportFilter filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets litter locations within a specified time range.
        /// </summary>
        /// <param name="startTime">The start of the time range.</param>
        /// <param name="endTime">The end of the time range.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of litter locations within the time range.</returns>
        Task<IEnumerable<Location>> GeLitterLocationsByTimeRangeAsync(DateTimeOffset? startTime,
            DateTimeOffset? endTime, CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a litter report by marking it as deleted.
        /// </summary>
        /// <param name="id">The unique identifier of the litter report.</param>
        /// <param name="userId">The unique identifier of the user performing the deletion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> DeleteAsync(Guid id, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Adds a new litter report with detailed result information.
        /// </summary>
        /// <param name="litterReport">The litter report to add.</param>
        /// <param name="userId">The unique identifier of the user creating the report.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A service result containing the created litter report or error details.</returns>
        Task<ServiceResult<LitterReport>> AddWithResultAsync(LitterReport litterReport, Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of all litter reports and cleaned litter reports efficiently.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A tuple containing (totalCount, cleanedCount).</returns>
        Task<(int TotalCount, int CleanedCount)> GetLitterReportCountsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of litter reports submitted by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The count of litter reports submitted by the user.</returns>
        Task<int> GetUserLitterReportCountAsync(Guid userId, CancellationToken cancellationToken = default);
    }
}
