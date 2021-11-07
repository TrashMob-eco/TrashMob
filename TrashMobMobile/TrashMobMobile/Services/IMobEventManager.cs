namespace TrashMobMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IMobEventManager
    {
        Task<IEnumerable<MobEvent>> GetEventsAsync();

        Task<MobEvent> GetEventAsync(Guid eventId);

        Task<MobEvent> UpdateEventAsync(MobEvent mobEvent);
    }
}