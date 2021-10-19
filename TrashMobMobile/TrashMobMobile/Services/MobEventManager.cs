using System.Collections.Generic;
using System.Threading.Tasks;
using TrashMobMobile.Models;

namespace TrashMobMobile.Services
{
    class MobEventManager
    {
        MobEventRestService mobEventRestService;
		public MobEventManager(MobEventRestService service)
		{
			mobEventRestService = service;
		}

        public Task<List<MobEvent>> GetTasksAsync()
		{
			return mobEventRestService.RefreshMobEventsAsync();
		}
	}
}
