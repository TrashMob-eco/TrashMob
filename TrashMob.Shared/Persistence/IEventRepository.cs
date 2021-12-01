namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventRepository
    {
        IQueryable<Event> GetEvents();

        Task<IEnumerable<Event>> GetAllEvents();
 
        Task<IEnumerable<Event>> GetActiveEvents();

        Task<IEnumerable<Event>> GetCompletedEvents();

        Task<IEnumerable<Event>> GetUserEvents(Guid userId, bool futureEventsOnly);

        Task<Event> AddEvent(Event mobEvent);

        Task<Event> UpdateEvent(Event mobEvent);

        Task<Event> GetEvent(Guid id);

        Task<int> DeleteEvent(Guid id, string cancellationReason);
    }
}
