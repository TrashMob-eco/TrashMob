namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
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

        /// <summary>
        /// Checks whether a user is an event lead for the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the user is an event lead; otherwise, false.</returns>
        Task<bool> IsEventLeadAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all event leads for the specified event.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of event attendees who are leads for the event.</returns>
        Task<IEnumerable<EventAttendee>> GetEventLeadsAsync(Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Promotes an attendee to event lead status.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="userId">The unique identifier of the user to promote.</param>
        /// <param name="promotedByUserId">The unique identifier of the user performing the promotion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event attendee with lead status.</returns>
        Task<EventAttendee> PromoteToLeadAsync(Guid eventId, Guid userId, Guid promotedByUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Demotes an event lead back to regular attendee status.
        /// </summary>
        /// <param name="eventId">The unique identifier of the event.</param>
        /// <param name="userId">The unique identifier of the user to demote.</param>
        /// <param name="demotedByUserId">The unique identifier of the user performing the demotion.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event attendee without lead status.</returns>
        Task<EventAttendee> DemoteFromLeadAsync(Guid eventId, Guid userId, Guid demotedByUserId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a queryable of active attendees for an event, including User navigation.
        /// The returned IQueryable is not materialized, allowing the caller to apply ToPagedAsync().
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <returns>An unmaterialized queryable of active event attendees with User included.</returns>
        IQueryable<EventAttendee> GetEventAttendeesQueryable(Guid eventId);

        /// <summary>
        /// Gets the count of active (non-canceled) attendees for an event.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The number of active attendees.</returns>
        Task<int> GetActiveAttendeeCountAsync(Guid eventId, CancellationToken cancellationToken = default);
    }
}
