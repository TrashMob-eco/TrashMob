namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class EventTypeRepository : IEventTypeRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventTypeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventType>> GetAllEventTypes()
        {
            return await mobDbContext.EventTypes
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
