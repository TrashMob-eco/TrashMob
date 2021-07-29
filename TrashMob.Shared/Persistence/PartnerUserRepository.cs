namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class PartnerUserRepository : IPartnerUserRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerUserRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        
        public async Task AddPartnerUser(PartnerUser partnerUser)
        {
            partnerUser.CreatedDate = DateTimeOffset.UtcNow;
            partnerUser.LastUpdatedByUserId = partnerUser.CreatedByUserId;
            partnerUser.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.PartnerUsers.Add(partnerUser);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public async Task<IEnumerable<PartnerUser>> GetPartnerUsers(Guid partnerId)
        {
            return await mobDbContext.PartnerUsers.Where(pu => pu.PartnerId == partnerId)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<Partner> GetPartnerUser(Guid partnerId, Guid userId)
        {
            return await mobDbContext.Partners.FindAsync(partnerId, userId);
        }

        // Update the records of a particular Partner User
        public async Task<PartnerUser> UpdatePartnerUser(PartnerUser partnerUser)
        {
            mobDbContext.Entry(partnerUser).State = EntityState.Modified;
            partnerUser.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync();

            return partnerUser;
        }

        public async Task<int> DeletePartnerUser(Guid partnerId, Guid userId)
        {
            var partnerUser = await mobDbContext.PartnerUsers.FindAsync(partnerId, userId).ConfigureAwait(false);
            mobDbContext.PartnerUsers.Remove(partnerUser);

            // Save the changes
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
