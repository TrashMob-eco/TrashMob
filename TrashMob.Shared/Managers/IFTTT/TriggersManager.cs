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

    internal class TriggersManager : BaseManager<IftttTrigger>, ITriggersManager
    {
        private readonly IKeyedRepository<Event> eventRepository;

        public TriggersManager(IBaseRepository<IftttTrigger> repository, IKeyedRepository<Event> eventRepository) : base(repository)
        {
            this.eventRepository = eventRepository;
        }

        public async Task<List<IftttEventResponse>> GetEventsTriggerDataAsync(TriggersRequest triggerRequest, Guid userId, CancellationToken cancellationToken)
        {
            if (triggerRequest == null)
            {
                throw new ArgumentNullException(nameof(triggerRequest));
            }

            // See if the trigger already exists
            var trigger = Repository.Get(t => t.TriggerId == triggerRequest.trigger_identity).FirstOrDefault();

            // Store Trigger in database if it does not exist
            if (trigger == null)       
            {
                trigger = new IftttTrigger()
                {
                    TriggerId = triggerRequest.trigger_identity,
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    TriggerFields = triggerRequest.triggerFields.ToString(),
                    IftttSource = triggerRequest.ifttt_source.ToString(),
                    Limit = triggerRequest.limit,
                    UserId = userId,
                };

                await Repository.AddAsync(trigger);
            }

            // Get the Slugs
            var eventFields = triggerRequest.triggerFields as IftttEventRequest;

            IQueryable<Event> events;

            // Return all events
            if (eventFields == null)
            {
                events = eventRepository.Get();
            }
            else
            {
                events = eventRepository.Get(e => (e.City == eventFields.City || string.IsNullOrWhiteSpace(eventFields.City)) &&
                                                      (e.Region == eventFields.Region || string.IsNullOrWhiteSpace(eventFields.Region)) &&
                                                      (e.Country == eventFields.Country || string.IsNullOrWhiteSpace(eventFields.Country)) &&
                                                      (e.PostalCode == eventFields.Postal_Code || string.IsNullOrWhiteSpace(eventFields.Postal_Code)));
            }

            
            var triggersResponses = new List<IftttEventResponse>();

            // Get all the public events in the future
            foreach (var mobEvent in events.Where(e => e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow).ToList().OrderByDescending(e => e.CreatedDate).Take(triggerRequest.limit))
            {
                var triggersResponse = new IftttEventResponse();
                triggersResponse.meta = new MetaResponse()
                {
                    id = mobEvent.Id.ToString(),
                    timestamp = mobEvent.CreatedDate.Value.ToUnixTimeSeconds()
                };

                triggersResponse.event_id = mobEvent.Id.ToString();
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
