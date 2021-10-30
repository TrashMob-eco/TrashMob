namespace TrashMobMobile.Services
{
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

        public Task<List<MobEvent>> GetEventsAsync()
        {
            return mobEventRestService.RefreshMobEventsAsync();
        }
    }
}

