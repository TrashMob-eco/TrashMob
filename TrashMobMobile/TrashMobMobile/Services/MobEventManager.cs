namespace TrashMobMobile.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public class MobEventManager
    {
        private readonly MobEventRestService mobEventRestService;

        public MobEventManager(MobEventRestService service)
        {
            mobEventRestService = service;
        }

        public Task<List<MobEvent>> GetEventsAsync()
        {
            return mobEventRestService.RefreshMobEventsAsync();
        }
    }
}

