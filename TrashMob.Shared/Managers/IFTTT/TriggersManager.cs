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
            foreach (var mobEvent in events.Where(e => e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow).ToList())
            {
                var triggersResponse = new IftttEventResponse();
                triggersResponse.meta = new MetaResponse()
                {
                    id = mobEvent.Id.ToString(),
                    timestamp = mobEvent.EventDate.Ticks
                };

                triggersResponse.Event_Name = mobEvent.Name;
                triggersResponse.EventDate = mobEvent.EventDate;
                triggersResponse.Street_Address = mobEvent.StreetAddress;
                triggersResponse.City = mobEvent.City;
                triggersResponse.Region = mobEvent.Region;
                triggersResponse.Country = mobEvent.Country;
                triggersResponse.Postal_Code = mobEvent.PostalCode;
                triggersResponse.Event_Details_Url = mobEvent.EventDetailsUrl();

                triggersResponses.Add(triggersResponse);
            }

            return triggersResponses;
        }
    }
}
