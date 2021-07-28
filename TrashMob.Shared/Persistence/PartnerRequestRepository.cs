namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class PartnerRequestRepository : IPartnerRequestRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerRequestRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        
        public async Task AddPartnerRequest(PartnerRequest partnerRequest)
        {
            partnerRequest.Id = Guid.NewGuid();
            partnerRequest.CreatedDate = DateTimeOffset.UtcNow;
            partnerRequest.LastUpdatedByUserId = partnerRequest.CreatedByUserId;
            partnerRequest.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.PartnerRequests.Add(partnerRequest);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<PartnerRequest>> GetPartnerRequests()
        {
            return await mobDbContext.PartnerRequests
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        // Update the records of a particular Partner Request
        public async Task<PartnerRequest> UpdatePartnerRequest(PartnerRequest partnerRequest)
        {
            mobDbContext.Entry(partnerRequest).State = EntityState.Modified;
            partnerRequest.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync();

            return partnerRequest;
        }
    }
}
