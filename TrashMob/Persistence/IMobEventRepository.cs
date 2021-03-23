namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IMobEventRepository
    {
        Task<IEnumerable<MobEvent>> GetAllMobEvents();

        Task<Guid> AddMobEvent(MobEvent mobEvent);

        Task<int> UpdateMobEvent(MobEvent mobEvent);

        Task<MobEvent> GetMobEvent(Guid id);

        Task<int> DeleteMobEvent(Guid id);
    }
}
