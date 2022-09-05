namespace TrashMobMobileApp.Data
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobileApp.Models;

    public interface IMobEventRestService
    {
        Task<IEnumerable<MobEvent>> GetActiveEventsAsync(CancellationToken cancellationToken = default);

        Task<IEnumerable<MobEvent>> GetUserEventsAsync(Guid userId, bool showFutureEventsOnly, CancellationToken cancellationToken = default);

        Task<MobEvent> GetEventAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<MobEvent> UpdateEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default);

        Task<MobEvent> AddEventAsync(MobEvent mobEvent, CancellationToken cancellationToken = default);

        Task DeleteEventAsync(CancelEvent cancelEvent, CancellationToken cancellationToken = default);

        Task<IEnumerable<MobEvent>> GetEventsUserIsAttending(Guid userId, CancellationToken cancellationToken = default);
    }
}