namespace TrashMob.Shared.Managers.Interfaces
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Poco;

    public interface IEventSummaryManager : IBaseManager<EventSummary>
    {
        Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter, CancellationToken cancellationToken);
    }
}
