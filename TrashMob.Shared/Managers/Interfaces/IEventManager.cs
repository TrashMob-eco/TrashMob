namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing events.
    /// </summary>
    public interface IEventManager : IKeyedManager<Event>
    {
        /// <summary>
        /// Gets all active events.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of active events.</returns>
        Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all completed events.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of completed events.</returns>
        Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all events created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="futureEventsOnly">Whether to return only future events.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events created by the user.</returns>
        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets events created by a specific user matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria for events.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events created by the user that match the filter.</returns>
        Task<IEnumerable<Event>> GetUserEventsAsync(EventFilter filter, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all canceled events created by a specific user.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="futureEventsOnly">Whether to return only future events.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of canceled events created by the user.</returns>
        Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets events matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria for events.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events matching the filter.</returns>
        Task<IEnumerable<Event>> GetFilteredEventsAsync(EventFilter filter,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event locations within a specified time range.
        /// </summary>
        /// <param name="startTime">The start of the time range.</param>
        /// <param name="endTime">The end of the time range.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of event locations within the time range.</returns>
        Task<IEnumerable<Location>> GeEventLocationsByTimeRangeAsync(DateTimeOffset? startTime, DateTimeOffset? endTime,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes an event by marking it as canceled.
        /// </summary>
        /// <param name="id">The unique identifier of the event.</param>
        /// <param name="cancellationReason">The reason for canceling the event.</param>
        /// <param name="userId">The unique identifier of the user performing the deletion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of entities deleted.</returns>
        Task<int> DeleteAsync(Guid id, string cancellationReason, Guid userId,
            CancellationToken cancellationToken = default);
    }
}
