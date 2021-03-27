namespace TrashMob.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;

    public class EventStatusRepository : IEventStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventStatus>> GetAllEventStatuses()
        {
            try
            {
                return await mobDbContext.EventStatuses.ToListAsync().ConfigureAwait(false);
            }
            catch
            {
                throw;
            }
        }
    }
}
