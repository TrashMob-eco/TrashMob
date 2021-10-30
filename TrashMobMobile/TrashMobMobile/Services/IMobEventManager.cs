using System.Collections.Generic;
using System.Threading.Tasks;
using TrashMobMobile.Models;

namespace TrashMobMobile.Services
{
    public interface IMobEventManager
    {
        Task<List<MobEvent>> GetEventsAsync();
    }
}