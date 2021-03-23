namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public class MobEventRepository : IMobEventRepository
    {
        private readonly MobDbContext mobDbContext;

        public MobEventRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<MobEvent>> GetAllMobEvents()
        {
            try
            {
                return await mobDbContext.MobEvents.ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        // Add new mobEvent record     
        public async Task<Guid> AddMobEvent(MobEvent mobEvent)
        {
            try
            {
                mobEvent.MobEventId = Guid.NewGuid();
                mobDbContext.MobEvents.Add(mobEvent);
                await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
                return mobEvent.MobEventId;
            }
            catch
            {
                throw;
            }
        }

        // Update the records of a particluar Mob Event  
        public Task<int> UpdateMobEvent(MobEvent mobEvent)
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

        // Get the details of a particular MobEvent    
        public async Task<MobEvent> GetMobEvent(Guid id)
        {
            try
            {
                return await mobDbContext.MobEvents.FindAsync(id).ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }

        // Delete the record of a particular Mob Event    
        public async Task<int> DeleteMobEvent(Guid id)
        {
            try
            {
                var mobEvent = await mobDbContext.MobEvents.FindAsync(id).ConfigureAwait(false);
                mobDbContext.MobEvents.Remove(mobEvent);
                return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }
    }
}
