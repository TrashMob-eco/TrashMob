namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Extensions;
    using TrashMob.Models;

    public class EventRepository : IEventRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<Event>> GetAllEvents()
        {
            return await mobDbContext.Events.ToListAsync().ConfigureAwait(false);
        }

        // Add new Event record     
        public async Task<Guid> AddEvent(Event mobEvent)
        {
            mobEvent.Id = Guid.NewGuid();
            mobEvent.EventStatusId = 1;
            mobDbContext.Events.Add(mobEvent);


            var eventHistory = mobEvent.ToEventHistory();
            mobDbContext.EventHistories.Add(eventHistory);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return mobEvent.Id;
        }

        // Update the records of a particluar Event  
        public Task<int> UpdateEvent(Event mobEvent)
        {
            mobDbContext.Entry(mobEvent).State = EntityState.Modified;
            var eventHistory = mobEvent.ToEventHistory();
            mobDbContext.EventHistories.Add(eventHistory);
            return mobDbContext.SaveChangesAsync();
        }

        // Get the details of a particular Event    
        public async Task<Event> GetEvent(Guid id)
        {
            return await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteEvent(Guid id)
        {
            var mobEvent = await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
            mobDbContext.Events.Remove(mobEvent);
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
