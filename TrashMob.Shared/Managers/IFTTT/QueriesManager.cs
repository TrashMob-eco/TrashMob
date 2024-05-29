namespace TrashMob.Shared.Managers.IFTTT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Shared.Poco.IFTTT;

    internal class QueriesManager : BaseManager<IftttTrigger>, IQueriesManager
    {
        private readonly IKeyedRepository<Event> eventRepository;

        public QueriesManager(IBaseRepository<IftttTrigger> repository, IKeyedRepository<Event> eventRepository) :
            base(repository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<List<IftttEventResponse>> GetEventsQueryDataAsync(QueriesRequest queryRequest, Guid userId,
            CancellationToken cancellationToken)
        {
            if (queryRequest == null)
            {
                throw new ArgumentNullException(nameof(queryRequest));
            }

            // See if the trigger already exists
            var trigger = Repository.Get(t => t.TriggerId == queryRequest.trigger_identity).FirstOrDefault();

            // Store Trigger in database if it does not exist
            if (trigger == null)
            {
                trigger = new IftttTrigger
                {
                    TriggerId = queryRequest.trigger_identity,
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    TriggerFields = queryRequest.triggerFields.ToString(),
                    IftttSource = queryRequest.ifttt_source.ToString(),
                    Limit = queryRequest.limit,
                    UserId = userId,
                };

                await Repository.AddAsync(trigger);
            }

            // Get the Slugs
            var eventFields = queryRequest.triggerFields as IftttEventRequest;

            IQueryable<Event> events;

            // Return all events
            if (eventFields == null)
            {
                events = eventRepository.Get();
            }
            else
            {
                events = eventRepository.Get(e =>
                    (e.City == eventFields.city || string.IsNullOrWhiteSpace(eventFields.city)) &&
                    (e.Region == eventFields.region || string.IsNullOrWhiteSpace(eventFields.region)) &&
                    (e.Country == eventFields.country || string.IsNullOrWhiteSpace(eventFields.country)) &&
                    (e.PostalCode == eventFields.postal_code || string.IsNullOrWhiteSpace(eventFields.postal_code)));
            }


            var triggersResponses = new List<IftttEventResponse>();

            // Get all the public events in the future
            foreach (var mobEvent in events.Where(e => e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow)
                         .ToList())
            {
                var triggersResponse = new IftttEventResponse();
                triggersResponse.meta = new MetaResponse
                {
                    id = mobEvent.Id.ToString(),
                    timestamp = mobEvent.EventDate.Ticks,
                };

                triggersResponse.event_name = mobEvent.Name;
                triggersResponse.event_date = mobEvent.EventDate;
                triggersResponse.street_address = mobEvent.StreetAddress;
                triggersResponse.city = mobEvent.City;
                triggersResponse.region = mobEvent.Region;
                triggersResponse.country = mobEvent.Country;
                triggersResponse.postal_code = mobEvent.PostalCode;
                triggersResponse.event_details_url = mobEvent.EventDetailsUrl();

                triggersResponses.Add(triggersResponse);
            }

            return triggersResponses;
        }
    }
}