namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Defines operations for managing events.
    /// </summary>
    public interface IEventManager : IKeyedManager<Event>
    {
        /// <summary>
        /// Gets all active events visible to the specified user.
        /// When userId is null, returns only public events.
        /// </summary>
        /// <param name="userId">Optional user ID to include team-only events visible to this user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of active events visible to the user.</returns>
        Task<IEnumerable<Event>> GetActiveEventsAsync(Guid? userId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all active team-only events (for notification engine use).
        /// Returns events with EventVisibilityId == TeamOnly, including Team navigation property.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of active team-only events.</returns>
        Task<IEnumerable<Event>> GetActiveTeamEventsAsync(CancellationToken cancellationToken = default);

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
        /// Gets events matching the specified filter criteria, respecting visibility rules.
        /// When userId is null, returns only public events.
        /// </summary>
        /// <param name="filter">The filter criteria for events.</param>
        /// <param name="userId">Optional user ID to include team-only events visible to this user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events matching the filter visible to the user.</returns>
        Task<IEnumerable<Event>> GetFilteredEventsAsync(EventFilter filter, Guid? userId = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets event locations within a specified time range.
        /// </summary>
        /// <param name="startTime">The start of the time range.</param>
        /// <param name="endTime">The end of the time range.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of event locations within the time range.</returns>
        Task<IEnumerable<Location>> GetEventLocationsByTimeRangeAsync(DateTimeOffset? startTime, DateTimeOffset? endTime,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a queryable of filtered events for V2 API pagination. The returned IQueryable
        /// is not materialized, allowing the caller to apply ToPagedAsync() for database-side pagination.
        /// </summary>
        /// <param name="filter">The V2 query parameters with event-specific filters.</param>
        /// <param name="userId">Optional user ID to include team-only events visible to this user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>An unmaterialized queryable of events matching the filter.</returns>
        Task<IQueryable<Event>> GetFilteredEventsQueryableAsync(EventQueryParameters filter, Guid? userId = null,
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

        /// <summary>
        /// Creates a new solo private Instant Event for the specified user at the given GPS
        /// coordinates. Auto-fills name (with timestamp), description, EventType (Cleanup),
        /// visibility (Private), status (Active), and EventDate (now). Registers the creator
        /// as sole attendee + event lead. Deliberately skips the info@ new-event notification
        /// email — private solo cleanups would flood that inbox.
        /// See Planning/Projects/Project_65_Instant_Events.md.
        /// </summary>
        /// <param name="latitude">Latitude of the user's location at Start.</param>
        /// <param name="longitude">Longitude of the user's location at Start.</param>
        /// <param name="userId">The user creating the event.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The newly created event.</returns>
        Task<Event> AddInstantEventAsync(double latitude, double longitude, Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an event as Complete and computes its actual duration from the elapsed time
        /// since the event was created. Used by the Instant Events "Stop" flow but works for
        /// any event. Duration is clamped to [0, 24h] as a safeguard against abandoned or
        /// backdated events producing absurd values.
        /// </summary>
        /// <param name="eventId">The event to complete.</param>
        /// <param name="userId">The user completing the event (used for audit fields).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated event.</returns>
        Task<Event> CompleteEventAsync(Guid eventId, Guid userId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns Instant Events owned by the specified user that appear to still be in
        /// progress — status Active, visibility Private, zero duration (never Completed),
        /// and started within the last 24 hours. Used by the mobile Dashboard to offer a
        /// resume path after a fresh install, cross-device switch, or cleared app data
        /// where the local Preferences record was lost. See
        /// Planning/Projects/Project_65_Instant_Events.md Phase 1 (Option B).
        /// </summary>
        Task<IEnumerable<Event>> GetInProgressInstantEventsAsync(Guid userId,
            CancellationToken cancellationToken = default);
    }
}
