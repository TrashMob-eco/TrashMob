namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventMediaRepository
    {
        Task<IEnumerable<EventMedia>> GetEventMedias();

        Task<IEnumerable<EventMedia>> GetEventMediasByEvent(Guid eventId);
 
        Task<IEnumerable<EventMedia>> GetEventMediasByUser(Guid userId);

        Task<EventMedia> GetEventMediaById(Guid eventMediaId);

        Task<EventMedia> AddUpdateEventMedia(EventMedia eventMedia);

        Task<int> DeleteEventMedia(Guid id);
    }
}
