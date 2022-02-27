namespace TrashMobMobile.Services
{
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IEventAttendeeRestService
    {
        Task AddAttendeeAsync(EventAttendee eventAttendee);
        Task RemoveAttendeeAsync(EventAttendee eventAttendee);
    }
}