namespace TrashMobMobile.Services
{
    using TrashMob.Models.Poco;

    public interface IEventAttendeeRouteRestService
    {
        Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesAsync(Guid eventId,
            Guid userId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForEventAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<DisplayEventAttendeeRoute>> GetEventAttendeeRoutesForUserAsync(Guid userId,
            CancellationToken cancellationToken = default);

        Task<DisplayEventAttendeeRoute> GetEventAttendeeRouteAsync(Guid id,
            CancellationToken cancellationToken = default);

        Task<DisplayEventAttendeeRoute> AddEventAttendeeRouteAsync(DisplayEventAttendeeRoute eventAttendeeRoute,
            CancellationToken cancellationToken = default);

        Task DeleteEventAttendeeRouteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}