using Microsoft.EntityFrameworkCore;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;
using TrashMob.Shared.Persistence.Interfaces;

namespace TrashMob.Shared.Persistence;

public class EventSummaryRepository : BaseRepository<EventSummary>, IEventSummaryRepository
{
    public EventSummaryRepository(MobDbContext mobDbContext) : base(mobDbContext)
    {
    }

    /// <summary>
    /// Get EventSummaries statistics
    /// </summary>
    /// <param name="cancellationToken">cancellation token</param>
    /// <returns></returns>
    public Task<EventStatistics> GetEventStatisticsAsync(CancellationToken cancellationToken)
    //We can also have this as db view
        => mobDbContext.Database.SqlQuery<EventStatistics>(@$"Select sum(es.NumberOfBags) + sum(es.NumberOfBuckets) / 3 as TotalBags,
sum(es.DurationInMinutes * es.ActualNumberOfAttendees / 60) as TotalHours,
sum(es.ActualNumberofAttendees) as TotalParticipants,
sum(CASE WHEN es. PickedWeight = 1 THEN es.PickedWeight ELSE CAST(es.PickedWeight * 2.20462 as INTEGER) END) as TotalWeightInPounds,
sum(CASE WHEN es. PickedWeight = 1 THEN CAST(es.PickedWeight * 0.453592 as INTEGER) ELSE es.PickedWeight END) as TotalWeightInKilograms
from  EventSummaries es").SingleAsync(cancellationToken);

}

public record EventStatistics(int TotalBags, int TotalHours, int TotalParticipants, int TotalWeightInPounds, int TotalWeightInKilograms);
