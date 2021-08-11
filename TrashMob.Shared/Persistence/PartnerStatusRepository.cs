namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class PartnerStatusRepository : IPartnerStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<PartnerStatus>> GetAllPartnerStatuses()
        {
            return await mobDbContext.PartnerStatus
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
