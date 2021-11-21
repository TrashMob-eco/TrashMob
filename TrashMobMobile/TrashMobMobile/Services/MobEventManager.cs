namespace TrashMobMobile.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class MobEventManager : IMobEventManager
    {
        private readonly IMobEventRestService mobEventRestService;

        public MobEventManager(IMobEventRestService service)
        {
            mobEventRestService = service;
        }

        public Task<IEnumerable<MobEvent>> GetEventsAsync()
        {
            return mobEventRestService.GetEventsAsync();
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
    }
}

