namespace TrashMob.Shared.Persistence.Events
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared;
    using System.Threading;
    using TrashMob.Shared.Persistence.Interfaces;
    using TrashMob.Models;

    public class EventRepository : IEventRepository
    {
        private readonly MobDbContext mobDbContext;
        private const int StandardEventWindowInMinutes = 120;

        public EventRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<Event>> GetAllEvents(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetActiveEvents(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events
                .Where(e => (e.EventStatusId == (int)EventStatusEnum.Active || e.EventStatusId == (int)EventStatusEnum.Full) && e.IsEventPublic && e.EventDate >= DateTimeOffset.UtcNow.AddMinutes(-1 * StandardEventWindowInMinutes))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCompletedEvents(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events
                .Where(e => e.EventDate < DateTimeOffset.UtcNow && e.EventStatusId != (int)EventStatusEnum.Canceled)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events
                .Where(e => e.CreatedByUserId == userId
                            && e.EventStatusId != (int)EventStatusEnum.Canceled
                            && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Event>> GetCanceledUserEvents(Guid userId, bool futureEventsOnly, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events
                .Where(e => e.CreatedByUserId == userId
                            && e.EventStatusId == (int)EventStatusEnum.Canceled
                            && (!futureEventsOnly || e.EventDate >= DateTimeOffset.UtcNow))
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Add new Event record     
        public async Task<Event> AddEvent(Event mobEvent)
        {
            mobEvent.Id = Guid.NewGuid();
            mobEvent.EventStatusId = (int)EventStatusEnum.Active;
            mobDbContext.Events.Add(mobEvent);

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
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.Events.FindAsync(mobEvent.Id).ConfigureAwait(false);
        }

        // Get the details of a particular Event    
        public async Task<Event> GetEvent(Guid id, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Events.AsNoTracking().SingleOrDefaultAsync(e => e.Id == id, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteEvent(Guid id, string cancellationReason)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
            mobEvent.EventStatusId = (int)EventStatusEnum.Canceled;
            mobEvent.CancellationReason = cancellationReason;
            mobDbContext.Entry(mobEvent).State = EntityState.Modified;
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public IQueryable<Event> GetEvents(CancellationToken cancellationToken = default)
        {
            return mobDbContext.Events.AsQueryable();
        }
    }
}
