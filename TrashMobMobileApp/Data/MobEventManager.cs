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

        public Task<IEnumerable<MobEvent>> GetActiveEventsAsync(CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetActiveEventsAsync(cancellationToken);
        }

        public Task<IEnumerable<MobEvent>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetUserEventsAsync(userId, showFutureEventsOnly, cancellationToken);
        }

        public Task<MobEvent> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetEventAsync(eventId, cancellationToken);
        }

        public Task<MobEvent> UpdateEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.UpdateEventAsync(mobEvent, cancellationToken);
        }

        public Task<MobEvent> AddEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.AddEventAsync(mobEvent, cancellationToken);
        }

        public Task DeleteEventAsync(CancelEvent cancelEvent, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.DeleteEventAsync(cancelEvent, cancellationToken);
        }

        public Task AddEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            return eventAttendeeRestService.AddAttendeeAsync(eventAttendee, cancellationToken);
        }

        public Task RemoveEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default)
        {
            return eventAttendeeRestService.RemoveAttendeeAsync(eventAttendee, cancellationToken);
        }

        public async Task<bool> IsUserAttendingAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default)
        {
            var events = await mobEventRestService.GetEventsUserIsAttending(userId, cancellationToken);

            return events != null && events.Any(e => e.Id == eventId);
        }

        public Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.GetEventSummaryAsync(eventId, cancellationToken);
        }

        public Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.UpdateEventSummaryAsync(eventSummary, cancellationToken);
        }

        public Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.AddEventSummaryAsync(eventSummary, cancellationToken);
        }
    }
}

