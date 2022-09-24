namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IMobEventManager
    {
        Task<IEnumerable<MobEvent>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<MobEvent>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default);

        Task<IEnumerable<MobEvent>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);

        Task<MobEvent> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<MobEvent> UpdateEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default);

        Task<MobEvent> AddEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(CancelEvent cancelEvent, CancellationToken cancellationToken = default);

        Task AddEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task RemoveEventAttendeeAsync(EventAttendee eventAttendee, CancellationToken cancellationToken = default);

        Task<bool> IsUserAttendingAsync(Guid eventId, Guid userId, CancellationToken cancellationToken = default);

        Task<EventSummary> GetEventSummaryAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<EventSummary> UpdateEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);

        Task<EventSummary> AddEventSummaryAsync(EventSummary eventSummary, CancellationToken cancellationToken = default);
    }
}