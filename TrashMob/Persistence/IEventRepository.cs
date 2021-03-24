namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventRepository
    {
        Task<IEnumerable<Event>> GetAllEvents();

        Task<Guid> AddEvent(Event mobEvent);

        Task<int> UpdateEvent(Event mobEvent);

        Task<Event> GetEvent(Guid id);

        Task<int> DeleteEvent(Guid id);
    }
}
