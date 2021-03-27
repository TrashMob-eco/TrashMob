namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventTypeRepository
    {
        Task<IEnumerable<EventType>> GetAllEventTypes();
    }
}
