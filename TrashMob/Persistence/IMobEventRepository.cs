using System;
using System.Collections.Generic;
using TrashMob.Models;

namespace TrashMob.Persistence
{
    public interface IMobEventRepository
    {
        IEnumerable<MobEvent> GetAllMobEvents();

        Guid AddMobEvent(MobEvent mobEvent);

        Guid UpdateMobEvent(MobEvent mobEvent);

        MobEvent GetMobEvent(Guid id);

        int DeleteMobEvent(Guid id);
    }
}
