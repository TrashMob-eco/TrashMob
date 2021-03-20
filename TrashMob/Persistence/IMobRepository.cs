namespace TrashMob.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IMobRepository
    {
        IQueryable<MobEvent> MobEvents { get; }

        IQueryable<Rsvp> Rsvp { get; }

        Task<MobEvent> GetMobEventAsync(Guid mobEventId);
        
        Task<List<MobEvent>> GetMobEventsAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng, int? pageIndex, int? pageSize);

        Task<List<MobEvent>> GetPopularMobEventsAsync();

        Task<MobEvent> CreateMobEventAsync(MobEvent item);

        Task<MobEvent> UpdateMobEventAsync(MobEvent mobEvent);

        Task DeleteMobEventAsync(Guid mobEventId);

        int GetMobEventsCount();

        Task<Rsvp> CreateRsvpAsync(MobEvent mobEvent, string userName);

        Task DeleteRsvpAsync(MobEvent mobEvent, string userName);
    }
}
