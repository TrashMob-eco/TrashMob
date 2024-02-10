namespace TrashMobMobile.Data
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventTypeRestService
    {
        Task<IEnumerable<EventType>> GetEventTypesAsync(CancellationToken cancellationToken = default);
    }
}