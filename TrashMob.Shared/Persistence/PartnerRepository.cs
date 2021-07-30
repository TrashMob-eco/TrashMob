namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class PartnerRepository : IPartnerRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        
        public async Task<Partner> AddPartner(Partner partner)
        {
            partner.Id = Guid.NewGuid();
            partner.CreatedDate = DateTimeOffset.UtcNow;
            partner.LastUpdatedByUserId = partner.CreatedByUserId;
            partner.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.Partners.Add(partner);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return partner;
        }

        public async Task<IEnumerable<Partner>> GetPartners()
        {
            return await mobDbContext.Partners
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<Partner> GetPartner(Guid id)
        {
            return await mobDbContext.Partners.FirstOrDefaultAsync(p => p.Id == id);
        }

        // Update the records of a particular Partner
        public async Task<Partner> UpdatePartner(Partner partner)
        {
            mobDbContext.Entry(partner).State = EntityState.Modified;
            partner.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync();

            return partner;
        }
    }
}
