namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventManager : IKeyedManager<Event>
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCompletedEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetCanceledUserEventsAsync(Guid userId, bool futureEventsOnly,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetFilteredEventsAsync(GeneralFilter filter,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<Location>> GeEventLocationsByTimeRangeAsync(DateTimeOffset? startTime, DateTimeOffset? endTime,
            CancellationToken cancellationToken = default);

        Task<int> DeleteAsync(Guid id, string cancellationReason, Guid userId,
            CancellationToken cancellationToken = default);
    }
}