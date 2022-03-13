namespace TrashMob.Shared.Persistence
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Shared.Models;

    public class WaiverDurationTypeRepository : IWaiverDurationTypeRepository
    {
        private readonly MobDbContext mobDbContext;

        public WaiverDurationTypeRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<WaiverDurationType>> GetAllWaiverDurationTypes(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.WaiverDurationTypes.Where(e => e.IsActive == true)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }
    }
}
