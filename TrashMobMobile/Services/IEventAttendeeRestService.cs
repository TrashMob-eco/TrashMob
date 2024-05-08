namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventAttendeeRestService
    {
        Task<IEnumerable<EventAttendee>> GetEventAttendeesAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);
    }
}