namespace TrashMobMobile.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMobMobile.Models;

    public interface IMobEventRestService
    {
        List<MobEvent> MobEvents { get; }

        Task<List<MobEvent>> RefreshMobEventsAsync();
    }
}