namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    /// <summary>
    /// Defines operations for managing event attendees.
    /// </summary>
    public interface IEventAttendeeManager : IBaseManager<EventAttendee>
    {
        /// <summary>
        /// Gets all events that a user is attending.
        /// </summary>
        /// <param name="attendeeId">The unique identifier of the attendee.</param>
        /// <param name="futureEventsOnly">Whether to return only future events. Defaults to false.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events the user is attending.</returns>
        Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(Guid attendeeId, bool futureEventsOnly = false,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets events that a user is attending matching the specified filter criteria.
        /// </summary>
        /// <param name="filter">The filter criteria for events.</param>
        /// <param name="attendeeId">The unique identifier of the attendee.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events the user is attending that match the filter.</returns>
        Task<IEnumerable<Event>> GetEventsUserIsAttendingAsync(EventFilter filter, Guid attendeeId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all canceled events that a user was attending.
        /// </summary>
        /// <param name="attendeeId">The unique identifier of the attendee.</param>
        /// <param name="futureEventsOnly">Whether to return only future events. Defaults to false.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of canceled events the user was attending.</returns>
        Task<IEnumerable<Event>> GetCanceledEventsUserIsAttendingAsync(Guid attendeeId, bool futureEventsOnly = false,
            CancellationToken cancellationToken = default);
    }
}
