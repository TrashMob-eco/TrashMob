namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class PartnerRequestStatusRepository : IPartnerRequestStatusRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerRequestStatusRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<PartnerRequestStatus>> GetAllPartnerRequestStatuses(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.PartnerRequestStatus
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }
    }
}
