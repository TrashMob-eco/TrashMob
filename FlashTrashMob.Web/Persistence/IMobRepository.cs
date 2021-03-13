namespace FlashTrashMob.Web.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FlashTrashMob.Web.Models;

    public interface IMobRepository
    {
        IQueryable<CleanupEvent> CleanupEvents { get; }

        IQueryable<Rsvp> Rsvp { get; }

        Task<CleanupEvent> GetCleanupEventAsync(int cleanupEventId);
        
        Task<List<CleanupEvent>> GetCleanupEventsAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng, int? pageIndex, int? pageSize);

        Task<List<CleanupEvent>> GetPopularCleanupEventsAsync();

        Task<CleanupEvent> CreateCleanupEventAsync(CleanupEvent item);

        Task<CleanupEvent> UpdateCleanupEventAsync(CleanupEvent cleanupEvent);

        Task DeleteCleanupEventAsync(int cleanupEventId);

        int GetCleanupEventsCount();

        Task<Rsvp> CreateRsvpAsync(CleanupEvent cleanupEvent, string userName);

        Task DeleteRsvpAsync(CleanupEvent cleanupEvent, string userName);
    }
}
