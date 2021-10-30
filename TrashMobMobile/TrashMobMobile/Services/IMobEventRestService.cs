using System.Collections.Generic;
using System.Threading.Tasks;
using TrashMobMobile.Models;

namespace TrashMobMobile.Services
{
    public interface IMobEventRestService
    {
        List<MobEvent> MobEvents { get; }

        Task<List<MobEvent>> RefreshMobEventsAsync();
    }
}