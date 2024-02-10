namespace TrashMobMobile.Data
{
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventAttendeeRouteRestService
    {
        Task AddAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute, CancellationToken cancellationToken = default);
        Task RemoveAttendeeRouteAsync(EventAttendeeRoute eventAttendeeRoute, CancellationToken cancellationToken = default);
    }
}