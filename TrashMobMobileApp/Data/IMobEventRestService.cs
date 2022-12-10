namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMobMobileApp.Models;

    public interface IMobEventRestService
    {
        Task<IEnumerable<Event>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default);

        Task<Event> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<Event> UpdateEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task<Event> AddEventAsync(Event mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(CancelEvent cancelEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<Event>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);
    }
}