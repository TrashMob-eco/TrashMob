using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;

namespace TrashMob.Shared.Persistence.Interfaces;

public interface IEventSummaryRepository : IBaseRepository<EventSummary>
{
    Task<EventStatistics> GetEventStatisticsAsync(CancellationToken cancellationToken);
}
