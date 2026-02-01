namespace TrashMob.Shared.Persistence.Interfaces;

using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;
using TrashMob.Shared.Poco;

public interface IEventSummaryRepository : IBaseRepository<EventSummary>
{
    Task<EventStatistics> GetEventStatisticsAsync(CancellationToken cancellationToken);
}
