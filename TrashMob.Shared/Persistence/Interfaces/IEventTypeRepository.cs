namespace TrashMob.Shared.Persistence.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventTypeRepository
    {
        Task<IEnumerable<EventType>> GetAllEventTypes(CancellationToken cancellationToken = default);
    }
}
