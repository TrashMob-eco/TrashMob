namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMobMobile.Models;

    public interface IMobEventRestService
    {
        Task<PaginatedList<Event>> GetFilteredEventsAsync(EventFilter filter,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly,
            CancellationToken cancellationToken = default);

        Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(EventCancellationRequest cancelEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
            DateTimeOffset endDate, CancellationToken cancellationToken = default);
    }
}