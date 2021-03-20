namespace TrashMob.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using TrashMob.Models;

    public class MobEventRepository : IMobEventRepository
    {
        private readonly MobDbContext mobDbContext;

        public MobEventRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public IEnumerable<MobEvent> GetAllMobEvents()
        {
            try
            {
                return mobDbContext.MobEvents.ToList();
            }
            catch
            {
                throw;
            }
        }

        // Add new mobEvent record     
        public Guid AddMobEvent(MobEvent mobEvent)
        {
            try
            {
                mobEvent.MobEventId = Guid.NewGuid();
                mobDbContext.MobEvents.Add(mobEvent);
                mobDbContext.SaveChanges();
                return mobEvent.MobEventId;
            }
            catch
            {
                throw;
            }
        }

        // Update the records of a particluar Mob Event  
        public Guid UpdateMobEvent(MobEvent mobEvent)
        {
            try
            {
                mobDbContext.Entry(mobEvent).State = EntityState.Modified;
                mobDbContext.SaveChanges();

                return mobEvent.MobEventId;
            }
            catch
            {
                throw;
            }
        }

        // Get the details of a particular MobEvent    
        public MobEvent GetMobEvent(Guid id)
        {
            try
            {
                return mobDbContext.MobEvents.Find(id);
            }
            catch
            {
                throw;
            }
        }

        // Delete the record of a particular Mob Event    
        public int DeleteMobEvent(Guid id)
        {
            try
            {
                var mobEvent = mobDbContext.MobEvents.Find(id);
                mobDbContext.MobEvents.Remove(mobEvent);
                mobDbContext.SaveChanges();
                return 1;
            }
            catch
            {
                throw;
            }
        }
    }
}
