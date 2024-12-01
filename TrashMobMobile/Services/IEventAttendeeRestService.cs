namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventAttendeeRestService
    {
        Task<IEnumerable<DisplayUser>> GetEventAttendeesAsync(Guid eventId,
            CancellationToken cancellationToken = default);

        Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);
    }
}