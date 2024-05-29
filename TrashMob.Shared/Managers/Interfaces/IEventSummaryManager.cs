namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

    public interface IEventSummaryManager : IBaseManager<EventSummary>
    {
        Task<Stats> GetStatsAsync(CancellationToken cancellationToken);

        Task<Stats> GetStatsByUser(Guid userId, CancellationToken cancellationToken);

        Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter,
            CancellationToken cancellationToken);
    }
}