namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public class MobEventManager : IMobEventManager
    {
        private readonly IMobEventRestService mobEventRestService;
        private readonly IEventSummaryRestService eventSummaryRestService;
        private readonly IEventAttendeeRestService eventAttendeeRestService;

        public MobEventManager(IMobEventRestService service, IEventAttendeeRestService eventAttendeeRestService, IEventSummaryRestService eventSummaryRestService)
        {
            mobEventRestService = service;
            this.eventAttendeeRestService = eventAttendeeRestService;
            this.eventSummaryRestService = eventSummaryRestService;
        }

        public Task<IEnumerable<MobEvent>> GetActiveEventsAsync()
        {
            return mobEventRestService.GetActiveEventsAsync();
        }

        public Task<IEnumerable<MobEvent>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly)
        {
            return mobEventRestService.GetUserEventsAsync(userId, showFutureEventsOnly);
        }

        public Task<MobEvent> GetEventAsync(Guid eventId)
        {
            return mobEventRestService.GetEventAsync(eventId);
        }

        public Task<MobEvent> UpdateEventAsync(MobEvent mobEvent)
        {
            return mobEventRestService.UpdateEventAsync(mobEvent);
        }

        public Task<MobEvent> AddEventAsync(MobEvent mobEvent)
        {
            return mobEventRestService.AddEventAsync(mobEvent);
        }

        public Task DeleteEventAsync(CancelEvent cancelEvent)
        {
            return mobEventRestService.DeleteEventAsync(cancelEvent);
        }

        public Task AddEventAttendeeAsync(EventAttendee eventAttendee)
        {
            return eventAttendeeRestService.AddAttendeeAsync(eventAttendee);
        }

        public Task RemoveEventAttendeeAsync(EventAttendee eventAttendee)
        {
            return eventAttendeeRestService.RemoveAttendeeAsync(eventAttendee);
        }

        public async Task<bool> IsUserAttendingAsync(Guid eventId, Guid userId)
        {
            var events = await mobEventRestService.GetEventsUserIsAttending(userId);

            return events != null && events.Any(e => e.Id == eventId);
        }

        public Task<EventSummary> GetEventSummaryAsync(Guid eventId)
        {
            return eventSummaryRestService.GetEventSummaryAsync(eventId);
        }

        public Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary)
        {
            return eventSummaryRestService.UpdateEventSummaryAsync(eventSummary);
        }

        public Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary)
        {
            return eventSummaryRestService.AddEventSummaryAsync(eventSummary);
        }
    }
}

