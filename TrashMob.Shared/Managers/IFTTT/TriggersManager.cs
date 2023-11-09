namespace TrashMob.Shared.Managers.IFTTT
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.Json;
    using System.Text.Json.Nodes;
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

        public async Task<List<IftttEventResponse>> GetEventsTriggerDataAsync(TriggersRequest triggersRequest, Guid userId, CancellationToken cancellationToken)
        {
            if (triggersRequest == null)
            {
                throw new ArgumentNullException(nameof(triggersRequest));
            }

            // See if the trigger already exists
            var trigger = Repository.Get(t => t.TriggerId == triggersRequest.trigger_identity).FirstOrDefault();

            // Store Trigger in database if it does not exist
            if (trigger == null)       
            {
                trigger = new IftttTrigger()
                {
                    TriggerId = triggersRequest.trigger_identity,
                    CreatedByUserId = userId,
                    LastUpdatedByUserId = userId,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdatedDate = DateTime.UtcNow,
                    TriggerFields = triggersRequest.triggerFields.ToString(),
                    IftttSource = triggersRequest.ifttt_source.ToString(),
                    Limit = triggersRequest.limit,
                    UserId = userId,
                };

                await Repository.AddAsync(trigger);
            }

            // Get the Slugs
            var eventFields = JsonSerializer.Deserialize<IftttEventRequest>(triggersRequest.triggerFields.ToString());

            IQueryable<Event> events;

            // Return all events
            if (eventFields == null)
            {
                events = eventRepository.Get();
            }
            else
            {
                events = eventRepository.Get(e => (e.City == eventFields.city || string.IsNullOrWhiteSpace(eventFields.city)) &&
                                                      (e.Region == eventFields.region || string.IsNullOrWhiteSpace(eventFields.region)) &&
                                                      (e.Country == eventFields.country || string.IsNullOrWhiteSpace(eventFields.country)) &&
                                                      (e.PostalCode == eventFields.postal_code || string.IsNullOrWhiteSpace(eventFields.postal_code)));
            }

            
            var triggersResponses = new List<IftttEventResponse>();

            // Get all the public events in the future
            foreach (var mobEvent in events.Where(e => e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow).ToList().OrderByDescending(e => e.CreatedDate).Take(triggersRequest.limit))
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

        public object ValidateRequest(TriggersRequest triggersRequest)
        {
            if (triggersRequest?.triggerFields == null)
            {
                var error = new
                {
                    errors = new JsonArray
                    {
                        new
                        {
                            error = "triggerFields missing from request body."
                        }
                    }
                };

                return error;
            }

            var eventFields = JsonSerializer.Deserialize<IftttEventRequest>(triggersRequest.triggerFields.ToString());

            if (eventFields.country == null || eventFields.city == null || eventFields.postal_code == null || eventFields.region == null)
            {
                var error = new
                {
                    errors = new JsonArray
                    {
                        new
                        {
                            error = "triggerFields must have city, region, country and postal_code."
                        }
                    }
                };

                return error;
            }

            return null;
        }

    }
}
