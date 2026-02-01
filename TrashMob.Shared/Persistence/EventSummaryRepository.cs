using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;
using TrashMob.Shared.Persistence.Interfaces;
using TrashMob.Shared.Poco;

namespace TrashMob.Shared.Persistence;

/// <summary>
/// Repository to get EventSummary data 
/// </summary>
public class EventSummaryRepository : BaseRepository<EventSummary>, IEventSummaryRepository
{
    /// <summary>
    /// Initializes a new instance of the <see cref="EventSummaryRepository"/> class.
    /// </summary>
    /// <param name="MobDbContext">db context</param>
    public EventSummaryRepository(MobDbContext mobDbContext)
        : base(mobDbContext)
    {
    }

    /// <summary>
    /// Get EventSummaries statistics
    /// </summary>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns>EventStatistics</returns>
    public Task<EventStatistics> GetEventStatisticsAsync(CancellationToken cancellationToken)
        => mobDbContext.Database.SqlQuery<EventStatistics>(@$"Select sum(es.NumberOfBags) + sum(es.NumberOfBuckets) / 3 as TotalBags,
sum(es.DurationInMinutes * es.ActualNumberOfAttendees / 60) as TotalHours,
sum(es.ActualNumberofAttendees) as TotalParticipants,
sum(CASE WHEN es. PickedWeight = 1 THEN CAST(es.PickedWeight as INTEGER) ELSE CAST(es.PickedWeight * 2.20462 as INTEGER) END) as TotalWeightInPounds,
sum(CASE WHEN es. PickedWeight = 1 THEN CAST(es.PickedWeight * 0.453592 as INTEGER) ELSE CAST(es.PickedWeight as INTEGER) END) as TotalWeightInKilograms
from  EventSummaries es").SingleAsync(cancellationToken);

}
