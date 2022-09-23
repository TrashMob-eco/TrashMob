namespace TrashMob.Shared.Persistence.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerRequestRepository : IPartnerRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<PartnerRequest> AddPartnerRequest(PartnerRequest partnerRequest)
        {
            partnerRequest.Id = Guid.NewGuid();
            partnerRequest.CreatedDate = DateTimeOffset.UtcNow;
            partnerRequest.LastUpdatedByUserId = partnerRequest.CreatedByUserId;
            partnerRequest.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.PartnerRequests.Add(partnerRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.PartnerRequests.FindAsync(partnerRequest.Id).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PartnerRequest>> GetPartnerRequests(CancellationToken cancellationToken = default)
        {
            return await mobDbContext.PartnerRequests
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<IEnumerable<PartnerRequest>> GetPartnerRequests(Guid userId, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.PartnerRequests.Where(r => r.CreatedByUserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<PartnerRequest> GetPartnerRequest(Guid id, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.PartnerRequests.FindAsync(new object[] { id }, cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        // Update the records of a particular Partner Request
        public async Task<PartnerRequest> UpdatePartnerRequest(PartnerRequest partnerRequest)
        {
            mobDbContext.Entry(partnerRequest).State = EntityState.Modified;
            partnerRequest.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.PartnerRequests.FindAsync(partnerRequest.Id).ConfigureAwait(false);
        }
    }
}
