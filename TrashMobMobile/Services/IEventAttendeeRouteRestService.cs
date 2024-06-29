namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IEventAttendeeRouteRestService
    {
        Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesAsync(Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesForEventAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventAttendeeRoute>> GetEventAttendeeRoutesForUserAsync(Guid userId,
            CancellationToken cancellationToken = default);

        Task<EventAttendeeRoute> GetEventAttendeeRouteAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task<EventAttendeeRoute> AddEventAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken = default);

        Task DeleteEventAttendeeRouteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}