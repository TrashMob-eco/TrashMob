﻿namespace TrashMobMobile.Services
{
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMobMobile.Models;

    public class MobEventManager : IMobEventManager
    {
        private readonly IEventAttendeeRestService eventAttendeeRestService;
        private readonly IEventSummaryRestService eventSummaryRestService;
        private readonly IMobEventRestService mobEventRestService;

        public MobEventManager(IMobEventRestService service, IEventAttendeeRestService eventAttendeeRestService,
            IEventSummaryRestService eventSummaryRestService)
        {
            mobEventRestService = service;
            this.eventAttendeeRestService = eventAttendeeRestService;
            this.eventSummaryRestService = eventSummaryRestService;
        }

        public Task<PaginatedList<Event>> GetFilteredEventsAsync(EventFilter filter, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetFilteredEventsAsync(filter, cancellationToken);
        }

        public Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly,
            CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetUserEventsAsync(userId, showFutureEventsOnly, cancellationToken);
        }

        public Task<PaginatedList<Event>> GetUserEventsAsync(EventFilter eventFilter, Guid userId, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetUserEventsAsync(eventFilter, userId, cancellationToken);
        }

        public Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken token)
        {
            return mobEventRestService.GetEventsUserIsAttending(userId, token);
        }

        public Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetEventAsync(eventId, cancellationToken);
        }

        public Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.UpdateEventAsync(mobEvent, cancellationToken);
        }

        public Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.AddEventAsync(mobEvent, cancellationToken);
        }

        public Task DeleteEventAsync(EventCancellationRequest cancelEvent,
            CancellationToken cancellationToken = default)
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

        public async Task<bool> IsUserAttendingAsync(Guid eventId, Guid userId,
            CancellationToken cancellationToken = default)
        {
            var events = await mobEventRestService.GetEventsUserIsAttending(userId, cancellationToken);

            return events != null && events.Any(e => e.Id == eventId);
        }

        public Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.GetEventSummaryAsync(eventId, cancellationToken);
        }

        public Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary,
            CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.UpdateEventSummaryAsync(eventSummary, cancellationToken);
        }

        public Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary,
            CancellationToken cancellationToken = default)
        {
            return eventSummaryRestService.AddEventSummaryAsync(eventSummary, cancellationToken);
        }

        public Task<IEnumerable<Location>> GetLocationsByTimeRangeAsync(DateTimeOffset startDate,
            DateTimeOffset endDate, CancellationToken cancellationToken = default)
        {
            return mobEventRestService.GetLocationsByTimeRangeAsync(startDate, endDate, cancellationToken);
        }
    }
}