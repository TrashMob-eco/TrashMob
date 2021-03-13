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

        Task<CleanupEvent> GetCleanupEventAsync(int dinnerId);
        
        Task<List<CleanupEvent>> GetCleanupEventsAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng, int? pageIndex, int? pageSize);

        Task<List<CleanupEvent>> GetPopularCleanupEventsAsync();

        Task<CleanupEvent> CreateCleanupEventAsync(CleanupEvent item);

        Task<CleanupEvent> UpdateCleanupEventAsync(CleanupEvent dinner);

        Task DeleteCleanupEventAsync(int dinnerId);

        int GetCleanupEventsCount();

        Task<Rsvp> CreateRsvpAsync(CleanupEvent dinner, string userName);

        Task DeleteRsvpAsync(CleanupEvent dinner, string userName);
    }
}
