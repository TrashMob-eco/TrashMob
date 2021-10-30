namespace TrashMobMobile.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IMobEventManager
    {
        Task<List<MobEvent>> GetEventsAsync();
    }
}