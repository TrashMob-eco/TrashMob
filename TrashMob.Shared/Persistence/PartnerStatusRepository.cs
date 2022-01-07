namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
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

        public async Task<IEnumerable<PartnerStatus>> GetAllPartnerStatuses(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.PartnerStatus
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
