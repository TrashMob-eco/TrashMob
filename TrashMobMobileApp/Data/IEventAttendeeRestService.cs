namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IEventAttendeeRestService
    {
        Task AddAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);
        Task RemoveAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);
    }
}