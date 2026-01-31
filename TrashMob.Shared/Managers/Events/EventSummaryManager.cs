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

            var eventSummaries = await Repository.Get().ToListAsync(cancellationToken);

            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) +
                              eventSummaries.Sum(es => es.NumberOfBuckets) / 3;
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes * es.ActualNumberOfAttendees / 60);
            stats.TotalParticipants = eventSummaries.Sum(es => es.ActualNumberOfAttendees);
            stats.TotalWeightInPounds = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight) +
                                       eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => (int)(e.PickedWeight * 2.20462));
            stats.TotalWeightInKilograms = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight) +
                                         eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => (int)(e.PickedWeight * 0.453592));

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
            var result2 = await eventAttendeeManager.GetEventsUserIsAttendingAsync(userId, false, cancellationToken)
                .ConfigureAwait(false);

            var allResults = result1.Union(result2, new EventComparer());

            stats.TotalEvents = allResults.Where(e => e.EventStatusId != CancelledEventStatusId).Count();
            var eventIds = allResults.Where(e => e.EventStatusId != CancelledEventStatusId).Select(e => e.Id);

            var eventSummaries =
                await Repository.Get(es => eventIds.Contains(es.EventId)).ToListAsync(cancellationToken);
            stats.TotalBags = eventSummaries.Sum(es => es.NumberOfBags) +
                              eventSummaries.Sum(es => es.NumberOfBuckets) / 3;
            stats.TotalHours = eventSummaries.Sum(es => es.DurationInMinutes) / 60;
            stats.TotalWeightInPounds = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => e.PickedWeight) +
                                       eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => (int)(e.PickedWeight * 2.20462));
            stats.TotalWeightInKilograms = eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Kilogram).Sum(e => e.PickedWeight) +
                                         eventSummaries.Where(e => e.PickedWeightUnitId == (int)WeightUnitEnum.Pound).Sum(e => (int)(e.PickedWeight * 0.453592));
            var eventLitterReports = await eventLitterReportManager.GetAsync(cancellationToken);

            if (eventLitterReports == null)
            {
                stats.TotalLitterReportsClosed = 0;
            }
            else
            {
                stats.TotalLitterReportsClosed = eventLitterReports.Count(elr => eventIds.Contains(elr.EventId) && elr.LitterReport != null && elr.LitterReport.LitterReportStatusId == (int)LitterReportStatusEnum.Cleaned);
            }

            stats.TotalLitterReportsSubmitted = await litterReportManager.GetUserLitterReportCountAsync(userId, cancellationToken);

            return stats;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter,
            CancellationToken cancellationToken = default)
        {
            var eventSummaries = Repository.Get();

            var displaySummaries = new List<DisplayEventSummary>();

            foreach (var eventSummary in eventSummaries)
            {
                var mobEvent = await eventRepository.GetAsync(eventSummary.EventId, cancellationToken)
                    .ConfigureAwait(false);

                if (mobEvent != null)
                {
                    if ((string.IsNullOrWhiteSpace(locationFilter.Country) || string.Equals(mobEvent.Country,
                            locationFilter.Country, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.Region) || string.Equals(mobEvent.Region,
                            locationFilter.Region, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.City) ||
                         mobEvent.City.Contains(locationFilter.City, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.PostalCode) ||
                         mobEvent.PostalCode.Contains(locationFilter.PostalCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        var displayEvent = new DisplayEventSummary
                        {
                            ActualNumberOfAttendees = eventSummary.ActualNumberOfAttendees,
                            City = mobEvent.City,
                            Country = mobEvent.Country,
                            DurationInMinutes = eventSummary.DurationInMinutes,
                            EventDate = mobEvent.EventDate,
                            EventId = mobEvent.Id,
                            EventTypeId = mobEvent.EventTypeId,
                            Name = mobEvent.Name,
                            NumberOfBags = eventSummary.NumberOfBags + eventSummary.NumberOfBuckets / 3.0,
                            PostalCode = mobEvent.PostalCode,
                            Region = mobEvent.Region,
                            StreetAddress = mobEvent.StreetAddress,
                            TotalWorkHours = eventSummary.ActualNumberOfAttendees * eventSummary.DurationInMinutes /
                                             60.0,
                        };

                        displaySummaries.Add(displayEvent);
                    }
                }
            }

            return displaySummaries;
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
            var result = await base.AddAsync(eventSummary, userId, cancellationToken).ConfigureAwait(false);

            // Mark all associated litter reports as Cleaned
            await MarkAssociatedLitterReportsAsCleanedAsync(eventSummary.EventId, userId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        /// <inheritdoc />
        public override async Task<EventSummary> UpdateAsync(EventSummary eventSummary, Guid userId, CancellationToken cancellationToken = default)
        {
            // Update the event summary
            var result = await base.UpdateAsync(eventSummary, userId, cancellationToken).ConfigureAwait(false);

            // Mark all associated litter reports as Cleaned (in case new associations were added)
            await MarkAssociatedLitterReportsAsCleanedAsync(eventSummary.EventId, userId, cancellationToken).ConfigureAwait(false);

            return result;
        }

        private async Task MarkAssociatedLitterReportsAsCleanedAsync(Guid eventId, Guid userId, CancellationToken cancellationToken)
        {
            // Get all litter reports associated with this event
            var eventLitterReports = await eventLitterReportManager.GetByParentIdAsync(eventId, cancellationToken).ConfigureAwait(false);

            foreach (var eventLitterReport in eventLitterReports)
            {
                if (eventLitterReport.LitterReport != null &&
                    eventLitterReport.LitterReport.LitterReportStatusId != (int)LitterReportStatusEnum.Cleaned)
                {
                    // Update the litter report status to Cleaned
                    eventLitterReport.LitterReport.LitterReportStatusId = (int)LitterReportStatusEnum.Cleaned;
                    await litterReportManager.UpdateAsync(eventLitterReport.LitterReport, userId, cancellationToken).ConfigureAwait(false);
                }
            }
        }
    }
}