namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
            try
            {
                return await mobDbContext.Events.ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        // Add new Event record     
        public async Task<Guid> AddEvent(Event mobEvent)
        {
            try
            {
                mobEvent.Id = Guid.NewGuid();
                mobDbContext.Events.Add(mobEvent);
                await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
                return mobEvent.Id;
            }
            catch
            {
                throw;
            }
        }

        // Update the records of a particluar Event  
        public Task<int> UpdateEvent(Event mobEvent)
        {
            try
            {
                mobDbContext.Entry(mobEvent).State = EntityState.Modified;
                return mobDbContext.SaveChangesAsync();
            }
            catch
            {
                throw;
            }
        }

        // Get the details of a particular Event    
        public async Task<Event> GetEvent(Guid id)
        {
            try
            {
                return await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteEvent(Guid id)
        {
            try
            {
                var mobEvent = await mobDbContext.Events.FindAsync(id).ConfigureAwait(false);
                mobDbContext.Events.Remove(mobEvent);
                return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }
    }
}
