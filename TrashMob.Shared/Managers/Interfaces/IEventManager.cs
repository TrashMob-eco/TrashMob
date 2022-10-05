namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventManager : IKeyedManager<Event>
    {
        Task<IEnumerable<Event>> GetActiveEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCompletedEvents(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCanceledUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default);

        Task<int> Delete(Guid id, string cancellationReason, CancellationToken cancellationToken);
    }
}
