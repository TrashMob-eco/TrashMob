namespace TrashMobMobileApp.Data
{
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IEventAttendeeRestService
    {
        Task AddAttendeeAsync(EventAttendee eventAttendee);
        Task RemoveAttendeeAsync(EventAttendee eventAttendee);
    }
}