namespace TrashMob.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IEventStatusRepository
    {
        Task<IEnumerable<EventStatus>> GetAllEventStatuses();
    }
}
