namespace TrashMobMobile.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobile.Models;

    public interface IMobEventManager
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);

        Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(EventCancellationRequest cancelEvent, CancellationToken cancellationToken = default);

        Task AddEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task RemoveEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task<bool> IsUserAttendingAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

        Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);

        Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);
    }
}