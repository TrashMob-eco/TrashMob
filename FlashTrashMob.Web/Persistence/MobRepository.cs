
namespace FlashTrashMob.Web.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Linq;
    using System.Threading.Tasks;
    using FlashTrashMob.Web.Models;

    public class MobRepository : IMobRepository
    {
        private readonly MobDbContext _database;

        public MobRepository(MobDbContext database)
        {
            _database = database;
        }

        public IQueryable<CleanupEvent> CleanupEvents => _database.CleanupEvents;

        public IQueryable<Rsvp> Rsvp => _database.Rsvp;

        public virtual async Task<CleanupEvent> GetCleanupEventAsync(int pickupEventId)
        {
            return await _database.CleanupEvents
                .Include(d => d.Rsvps)
                .SingleOrDefaultAsync(d => d.CleanupEventId == pickupEventId);
        }        

        public virtual async Task<List<CleanupEvent>> GetCleanupEventsAsync(DateTime? startDate, DateTime? endDate, string userName, string searchQuery, string sort, bool descending, double? lat, double? lng, int? pageIndex, int? pageSize)
        {
            var query = _database.CleanupEvents.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userName))
            {
                query = query.Where(d => string.Equals(d.UserName, userName, StringComparison.OrdinalIgnoreCase));
            }

            if (startDate.HasValue)
            {
                query = query.Where(d => d.EventDate >= startDate.Value);
            }
            else
            {
                query = query.Where(d => d.EventDate >= DateTime.Now);
            }

            if (endDate.HasValue)
            {
                query = query.Where(d => d.EventDate <= endDate.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                query = query.Where(
                    d => d.Title.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1 ||
                    d.Description.IndexOf(searchQuery, StringComparison.OrdinalIgnoreCase) != -1);
            }

            if (lat.HasValue)
            {
                query = query.Where(d => d.Latitude == lat.Value);
            }

            if (lng.HasValue)
            {
                query = query.Where(d => d.Longitude == lng.Value);
            }

            query = ApplyCleanupEventSort(query, sort, descending);

            if(pageIndex.HasValue && pageSize.HasValue)
            {
                query = query.Skip((pageIndex.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        public virtual async Task<List<CleanupEvent>> GetPopularCleanupEventsAsync()
        {
            return await _database.CleanupEvents
                .Include(d => d.Rsvps)
                .OrderByDescending(d => d.Rsvps.Count)
                .Take(8)
                .ToListAsync();
        }

        public virtual async Task<CleanupEvent> CreateCleanupEventAsync(CleanupEvent pickupEvent)
        {
            var rsvp = new Rsvp
            {
                UserName = pickupEvent.UserName
            };

            pickupEvent.Rsvps = new List<Rsvp> { rsvp };

            _database.Add(pickupEvent);
            _database.Add(rsvp);
            await _database.SaveChangesAsync();

            return pickupEvent;
        }

        public virtual async Task<CleanupEvent> UpdateCleanupEventAsync(CleanupEvent pickupEvent)
        {
            _database.Update(pickupEvent);
            await _database.SaveChangesAsync();
            return pickupEvent;
        }

        public virtual async Task DeleteCleanupEventAsync(int pickupEventId)
        {
            var pickupEvent = await GetCleanupEventAsync(pickupEventId);
            if (pickupEvent != null)
            {
                foreach (Rsvp rsvp in pickupEvent.Rsvps)
                {
                    _database.Rsvp.Remove(rsvp);
                }

                _database.CleanupEvents.Remove(pickupEvent);

                await _database.SaveChangesAsync();
            }

            // Else no errors - this operation is idempotent
        }

        public virtual int GetCleanupEventsCount()
        {
            return _database.CleanupEvents.Where(d => d.EventDate >= DateTime.Now).Count();
        }

        public virtual async Task<Rsvp> CreateRsvpAsync(CleanupEvent pickupEvent, string userName)
        {
            Rsvp rsvp = null;
            if (pickupEvent != null)
            {
                if (pickupEvent.IsUserRegistered(userName))
                {
                    rsvp = pickupEvent.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
                }
                else
                {
                    rsvp = new Rsvp
                    {
                        UserName = userName
                    };

                    pickupEvent.Rsvps.Add(rsvp);
                    _database.Add(rsvp);
                    await _database.SaveChangesAsync();
                }
            }

            return rsvp;
        }

        public virtual async Task DeleteRsvpAsync(CleanupEvent pickupEvent, string userName)
        {
            var rsvp = pickupEvent?.Rsvps.SingleOrDefault(r => string.Equals(r.UserName, userName, StringComparison.OrdinalIgnoreCase));
            if (rsvp != null)
            {
                _database.Rsvp.Remove(rsvp);
                await _database.SaveChangesAsync();
            };

            // Else no errors - this operation is idempotent
        }

        private IQueryable<CleanupEvent> ApplyCleanupEventSort(IQueryable<CleanupEvent> query, string sort, bool descending)
        {
            // Default to sort by pickupEvent Id
            query = descending ? query.OrderByDescending(d => d.CleanupEventId) : query.OrderBy(d => d.CleanupEventId);

            if (string.Equals(sort, "Title", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.Title) : query.OrderBy(d => d.Title);
            }
            else if (string.Equals(sort, "EventDate", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.EventDate) : query.OrderBy(d => d.EventDate);
            }
            else if (string.Equals(sort, "UserName", StringComparison.OrdinalIgnoreCase))
            {
                query = descending ? query.OrderByDescending(d => d.UserName) : query.OrderBy(d => d.UserName);
            }

            return query;
        }
    }
}
