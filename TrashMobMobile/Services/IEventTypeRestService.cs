namespace TrashMobMobile.Services
{
    using TrashMob.Models;

    public interface IEventTypeRestService
    {
        Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default);
    }
}