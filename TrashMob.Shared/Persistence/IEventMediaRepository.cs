namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IEventMediaRepository
    {
        Task<IEnumerable<EventMedia>> GetEventMedias(CancellationToken cancellationToken = default);

        Task<IEnumerable<EventMedia>> GetEventMediasByEvent(Guid eventId, CancellationToken cancellationToken = default);
 
        Task<IEnumerable<EventMedia>> GetEventMediasByUser(Guid userId, CancellationToken cancellationToken = default);

        Task<EventMedia> GetEventMediaById(Guid eventMediaId, CancellationToken cancellationToken = default);

        Task<EventMedia> AddUpdateEventMedia(EventMedia eventMedia);

        Task<int> DeleteEventMedia(Guid id);
    }
}
