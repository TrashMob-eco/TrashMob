namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class EventStatusRepository : IEventStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventStatus>> GetAllEventStatuses()
        {
            return await mobDbContext.EventStatuses
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
