namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class WaiverRepository : IWaiverRepository
    {
        private readonly MobDbContext mobDbContext;

        public WaiverRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }
        
        public async Task<Waiver> AddWaiver(Waiver waiver)
        {
            waiver.Id = Guid.NewGuid();
            waiver.CreatedDate = DateTimeOffset.UtcNow;
            waiver.LastUpdatedByUserId = waiver.CreatedByUserId;
            waiver.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.Waivers.Add(waiver);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.Waivers.FindAsync(waiver.Id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Waiver>> GetWaivers(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Waivers
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<Waiver> GetWaiver(Guid id, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.Waivers.FindAsync(new object[] { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Update the records of a particular Waiver
        public async Task<Waiver> UpdateWaiver(Waiver waiver)
        {
            mobDbContext.Entry(waiver).State = EntityState.Modified;
            waiver.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.Waivers.FindAsync(waiver.Id).ConfigureAwait(false);
        }
    }
}
