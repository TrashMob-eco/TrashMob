namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class EventPartnerStatusRepository : IEventPartnerStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventPartnerStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventPartnerStatus>> GetAllEventPartnerStatuses()
        {
            return await mobDbContext.EventPartnerStatuses
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
