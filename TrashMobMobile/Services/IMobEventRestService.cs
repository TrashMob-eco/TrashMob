namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMobMobile.Models;

    public interface IMobEventRestService
    {
        Task<PaginatedList<Event>> GetFilteredEventsAsync(EventFilter filter,
            CancellationToken cancellationToken = default);

        Task<PaginatedList<Event>> GetUserEventsAsync(EventFilter eventFilter, Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly,
            CancellationToken cancellationToken = default);

        Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(EventCancellationRequest cancelEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
            DateTimeOffset endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Creates a new Instant Event (solo private cleanup) at the given GPS coordinates.
        /// Server auto-fills name, description, event type, visibility, and status. See
        /// Planning/Projects/Project_65_Instant_Events.md.
        /// </summary>
        Task<Event> AddInstantEventAsync(double latitude, double longitude,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Marks an event as Complete and computes its duration from elapsed time.
        /// </summary>
        Task<Event> CompleteEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Returns the caller's Instant Events that still look in-progress on the server
        /// (Active + Private + zero duration + started within the last 24 hours). Used to
        /// offer a cross-device / fresh-install resume path when local Preferences is empty.
        /// </summary>
        Task<IEnumerable<Event>> GetInProgressInstantEventsAsync(
            CancellationToken cancellationToken = default);
    }
}