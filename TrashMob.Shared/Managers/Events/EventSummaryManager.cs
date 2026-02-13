namespace TrashMob.Shared.Managers.Events
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Migrations;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages event summaries including statistics, post-event reports, and impact metrics.
    /// </summary>
    public class EventSummaryManager : BaseManager<EventSummary>, IEventSummaryManager
    {
        private readonly IEventAttendeeManager eventAttendeeManager;
        private readonly ILitterReportManager litterReportManager;
        private readonly IEventLitterReportManager eventLitterReportManager;
        private readonly IEventManager eventManager;
        private readonly IKeyedRepository<Event> eventRepository;
        private const int CancelledEventStatusId = 3;

        /// <summary>
        /// Initializes a new instance of the <see cref="EventSummaryManager"/> class.
        /// </summary>
        public EventSummaryManager(IBaseRepository<EventSummary> repository,
            IKeyedRepository<Event> eventRepository,
            IEventManager eventManager,
            IEventAttendeeManager eventAttendeeManager,
            ILitterReportManager litterReportManager,
            IEventLitterReportManager eventLitterReportManager) 
            : base(repository)
        {
            this.eventRepository = eventRepository;
            this.eventManager = eventManager;
            this.eventAttendeeManager = eventAttendeeManager;
            this.litterReportManager = litterReportManager;
            this.eventLitterReportManager = eventLitterReportManager;
        }

        /// <inheritdoc />
        public async Task<Stats> GetStatsAsync(CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var events = eventRepository.Get();
            stats.TotalEvents = await events.CountAsync(e => e.EventStatusId != CancelledEventStatusId, cancellationToken);

            // Use database-side aggregation instead of loading all records into memory
            var aggregates = await Repository.Get()
                .GroupBy(es => 1)
                .Select(g => new
                {
                    TotalBags = g.Sum(es => es.NumberOfBags),
                    TotalBuckets = g.Sum(es => es.NumberOfBuckets),
                    TotalHoursMinutes = g.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees),
                    TotalParticipants = g.Sum(es => es.ActualNumberOfAttendees),
                    WeightPounds = g.Sum(es => es.PickedWeightUnitId == (int)WeightUnitEnum.Pound ? es.PickedWeight : 0),
                    WeightKilograms = g.Sum(es => es.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram ? es.PickedWeight : 0),
                })
                .FirstOrDefaultAsync(cancellationToken);

            if (aggregates != null)
            {
                stats.TotalBags = aggregates.TotalBags + aggregates.TotalBuckets / 3;
                stats.TotalHours = aggregates.TotalHoursMinutes / 60;
                stats.TotalParticipants = aggregates.TotalParticipants;
                stats.TotalWeightInPounds = aggregates.WeightPounds + aggregates.WeightKilograms * 2.20462m;
                stats.TotalWeightInKilograms = aggregates.WeightKilograms + aggregates.WeightPounds * 0.453592m;
            }

            var (totalLitterReports, cleanedLitterReports) = await litterReportManager.GetLitterReportCountsAsync(cancellationToken);
            stats.TotalLitterReportsClosed = cleanedLitterReports;
            stats.TotalLitterReportsSubmitted = totalLitterReports;

            return stats;
        }

        /// <inheritdoc />
        public async Task<Stats> GetStatsByUser(Guid userId, CancellationToken cancellationToken)
        {
            var stats = new Stats();
            var result1 = await eventManager.GetUserEventsAsync(userId, false, cancellationToken);
            var result2 = await eventAttendeeManager.GetEventsUserIsAttendingAsync(userId, false, cancellationToken);

            var allResults = result1.Union(result2, new EventComparer());

            stats.TotalEvents = allResults.Where(e => e.EventStatusId != CancelledEventStatusId).Count();
            var eventIds = allResults.Where(e => e.EventStatusId != CancelledEventStatusId).Select(e => e.Id);

            var eventSummaries =
                await Repository.Get(es => eventIds.Contains(es.EventId)).ToListAsync(cancellationToken);
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) +
                              eventSummaries.Sum(es => es.NumberOfBuckets) / 3;
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes) / 60;
            stats.TotalWeightInPounds = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight) +
                                       eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight * 2.20462m);
            stats.TotalWeightInKilograms = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight) +
                                         eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight * 0.453592m);
            // Use the expression-based GetAsync which includes the LitterReport navigation property,
            // and filter by the user's event IDs to avoid loading ALL event litter reports
            var eventIdsList = eventIds.ToList();
            var eventLitterReports = await eventLitterReportManager.GetAsync(
                elr => eventIdsList.Contains(elr.EventId), cancellationToken);

            stats.TotalLitterReportsClosed = eventLitterReports.Count(elr =>
                elr.LitterReport != null &&
                elr.LitterReport.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned);

            stats.TotalLitterReportsSubmitted = await litterReportManager.GetUserLitterReportCountAsync(userId, cancellationToken);

            return stats;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter,
            CancellationToken cancellationToken = default)
        {
            // Load all event summaries in a single query
            var eventSummaries = await Repository.Get().ToListAsync(cancellationToken);

            if (!eventSummaries.Any())
            {
                return Enumerable.Empty<DisplayEventSummary>();
            }

            // Build filtered event query with location criteria (single query instead of N+1)
            var summaryEventIds = eventSummaries.Select(es => es.EventId).Distinct().ToList();
            var eventQuery = eventRepository.Get().Where(e => summaryEventIds.Contains(e.Id));

            if (!string.IsNullOrWhiteSpace(locationFilter.Country))
                eventQuery = eventQuery.Where(e => e.Country == locationFilter.Country);

            if (!string.IsNullOrWhiteSpace(locationFilter.Region))
                eventQuery = eventQuery.Where(e => e.Region == locationFilter.Region);

            if (!string.IsNullOrWhiteSpace(locationFilter.City))
                eventQuery = eventQuery.Where(e => e.City.Contains(locationFilter.City));

            if (!string.IsNullOrWhiteSpace(locationFilter.PostalCode))
                eventQuery = eventQuery.Where(e => e.PostalCode.Contains(locationFilter.PostalCode));

            var events = await eventQuery.ToDictionaryAsync(e => e.Id, cancellationToken);

            // Join in memory and project to display models
            return eventSummaries
                .Where(es => events.ContainsKey(es.EventId))
                .Select(es =>
                {
                    var mobEvent = events[es.EventId];
                    return new DisplayEventSummary
                    {
                        ActualNumberOfAttendees = es.ActualNumberOfAttendees,
                        City = mobEvent.City,
                        Country = mobEvent.Country,
                        DurationInMinutes = es.DurationInMinutes,
                        EventDate = mobEvent.EventDate,
                        EventId = mobEvent.Id,
                        EventTypeId = mobEvent.EventTypeId,
                        Name = mobEvent.Name,
                        NumberOfBags = es.NumberOfBags + es.NumberOfBuckets / 3.0,
                        PostalCode = mobEvent.PostalCode,
                        Region = mobEvent.Region,
                        StreetAddress = mobEvent.StreetAddress,
                        TotalWorkHours = es.ActualNumberOfAttendees * es.DurationInMinutes / 60.0,
                    };
                })
                .ToList();
        }

        /// <inheritdoc />
        public override async Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken)
        {
            var eventSummary =
                await Repository.Get(es => es.EventId == parentId).FirstOrDefaultAsync(cancellationToken);
            return await Repository.DeleteAsync(eventSummary);
        }

        /// <inheritdoc />
        public override async Task<EventSummary> AddAsync(EventSummary eventSummary, Guid userId, CancellationToken cancellationToken = default)
        {
            // Add the event summary first
            var result = await base.AddAsync(eventSummary, userId, cancellationToken);

            // Mark all associated litter reports as Cleaned
            await MarkAssociatedLitterReportsAsCleanedAsync(eventSummary.EventId, userId, cancellationToken);

            return result;
        }

        /// <inheritdoc />
        public override async Task<EventSummary> UpdateAsync(EventSummary eventSummary, Guid userId, CancellationToken cancellationToken = default)
        {
            // Update the event summary
            var result = await base.UpdateAsync(eventSummary, userId, cancellationToken);

            // Mark all associated litter reports as Cleaned (in case new associations were added)
            await MarkAssociatedLitterReportsAsCleanedAsync(eventSummary.EventId, userId, cancellationToken);

            return result;
        }

        private async Task MarkAssociatedLitterReportsAsCleanedAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Get all litter reports associated with this event
            var eventLitterReports = await eventLitterReportManager.GetByParentIdAsync(eventId, cancellationToken);

            foreach (var eventLitterReport in eventLitterReports)
            {
                if (eventLitterReport.LitterReport != null &&
                    eventLitterReport.LitterReport.LitterReportStatusId != (int)LitterReportStatusEnum.Cleaned)
                {
                    // Update the litter report status to Cleaned
                    eventLitterReport.LitterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Cleaned;
                    await litterReportManager.UpdateAsync(eventLitterReport.LitterReport, userId, cancellationToken);
                }
            }
        }
    }
}