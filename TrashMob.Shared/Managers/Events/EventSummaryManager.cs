namespace TrashMob.Shared.Managers.Events
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventSummaryManager : BaseManager<EventSummary>,  IEventSummaryManager
    {
        private readonly IKeyedRepository<Event> eventRepository;

        public EventSummaryManager(IBaseRepository<EventSummary> repository, IKeyedRepository<Event> eventRepository) : base(repository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<IEnumerable<DisplayEventSummary>> GetFilteredAsync(LocationFilter locationFilter, CancellationToken cancellationToken = default)
        {
            var eventSummaries = Repository.Get();

            var displaySummaries = new List<DisplayEventSummary>();

            foreach (var eventSummary in eventSummaries)
            {
                var mobEvent = await eventRepository.GetAsync(eventSummary.EventId, cancellationToken).ConfigureAwait(false);

                if (mobEvent != null)
                {
                    if ((string.IsNullOrWhiteSpace(locationFilter.Country) || string.Equals(mobEvent.Country, locationFilter.Country, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.Region) || string.Equals(mobEvent.Region, locationFilter.Region, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.City) || mobEvent.City.Contains(locationFilter.City, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrWhiteSpace(locationFilter.PostalCode) || mobEvent.PostalCode.Contains(locationFilter.PostalCode, StringComparison.OrdinalIgnoreCase)))
                    {
                        var displayEvent = new DisplayEventSummary()
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
                            TotalWorkHours = eventSummary.ActualNumberOfAttendees * eventSummary.DurationInMinutes / 60.0
                        };

                        displaySummaries.Add(displayEvent);
                    }
                }
            }

            return displaySummaries;
        }

        public override async Task<int> DeleteAsync(Guid parentId, CancellationToken cancellationToken)
        {
            var eventSummary = await Repository.Get(es => es.EventId == parentId).FirstOrDefaultAsync(cancellationToken);
            return await Repository.DeleteAsync(eventSummary, cancellationToken);
        }
    }
}
