namespace TrashMob.Shared.Persistence.Events
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class EventStatusRepository : IEventStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public EventStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<EventStatus>> GetAllEventStatuses(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.EventStatuses
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
