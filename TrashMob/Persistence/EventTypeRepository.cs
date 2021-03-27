namespace TrashMob.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;

    public class EventTypeRepository : IEventTypeRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventTypeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventType>> GetAllEventTypes()
        {
            try
            {
                return await mobDbContext.EventTypes.ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }
    }
}
