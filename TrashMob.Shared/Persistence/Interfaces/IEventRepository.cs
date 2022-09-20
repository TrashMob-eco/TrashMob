namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventRepository
    {
        IQueryable<Event> GetEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetAllEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetActiveEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCompletedEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCanceledUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default);

        Task<Event> AddEvent(Event mobEvent);

        Task<Event> UpdateEvent(Event mobEvent);

        Task<Event> GetEvent(Guid id, CancellationToken cancellationToken = default);

        Task<int> DeleteEvent(Guid id, string cancellationReason);
    }
}
