namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Extensions;
    using TrashMob.Shared.Models;
    using TrashMob.Shared;

    public class EventRepository : IEventRepository
    {
        private readonly MobDbContext mobDbContext;
        private const int StandardEventWindowInMinutes = 120;

        public EventRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await mobDbContext.Events
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetActiveEvents()
        {
            return await mobDbContext.Events
                .Where(e => (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full) && e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes))
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCompletedEvents()
        {
            return await mobDbContext.Events
                .Where(e => e.EventDate < DateTimeOffset.UtcNow)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetUserEvents(Guid userId, bool futureEventsOnly)
        {
            return await mobDbContext.Events
                .Where(e => e.CreatedByUserId == userId
                            && e.EventStatusId != (int)EventStatusEnum.Canceled 
                            && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        // Add new Event record     
        public async Task<Event> AddEvent(Event mobEvent)
        {
            mobEvent.Id = Guid.NewGuid();
            mobEvent.EventStatusId = (int)EventStatusEnum.Active;
            mobDbContext.Events.Add(mobEvent);

            var eventHistory = mobEvent.ToEventHistory();
            mobDbContext.EventHistories.Add(eventHistory);

            var newAttendee = new EventAttendee
            {
                EventId = mobEvent.Id,
                UserId = mobEvent.CreatedByUserId,
                SignUpDate = DateTimeOffset.UtcNow
            };
            
            mobDbContext.EventAttendees.Add(newAttendee);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.Events.FindAsync(mobEvent.Id).ConfigureAwait(false);
        }

        // Update the records of a particular Event  
        public async Task<Event> UpdateEvent(Event mobEvent)
        {
            mobDbContext.Entry(mobEvent).State = EntityState.Modified;
            var eventHistory = mobEvent.ToEventHistory();
            mobDbContext.EventHistories.Add(eventHistory);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.Events.FindAsync(mobEvent.Id).ConfigureAwait(false);
        }

        // Get the details of a particular Event    
        public async Task<Event> GetEvent(Guid id)
        {
            return await mobDbContext.Events.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteEvent(Guid id)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
            mobEvent.EventStatusId = (int)EventStatusEnum.Canceled;
            mobDbContext.Entry(mobEvent).State = EntityState.Modified;
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public IQueryable<Event> GetEvents()
        {
            return mobDbContext.Events.AsQueryable();
        }
    }
}
