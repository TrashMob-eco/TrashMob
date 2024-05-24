namespace TrashMobMobile.Data
{
    using TrashMob.Models;
    using TrashMobMobile.Models;

    public interface IMobEventRestService
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetAllEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly,
            CancellationToken cancellationToken = default);

        Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(EventCancellationRequest cancelEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);

        Task<IEnumerable<TrashMob.Models.Poco.Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate, CancellationToken cancellationToken = default);
    }
}