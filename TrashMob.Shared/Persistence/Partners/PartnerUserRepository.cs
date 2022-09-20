namespace TrashMob.Shared.Persistence.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerUserRepository : IPartnerUserRepository
    {
        private readonly MobDbContext mobDbContext;

        public PartnerUserRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<PartnerUser> AddPartnerUser(PartnerUser partnerUser)
        {
            partnerUser.CreatedDate = DateTimeOffset.UtcNow;
            partnerUser.LastUpdatedByUserId = partnerUser.CreatedByUserId;
            partnerUser.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.PartnerUsers.Add(partnerUser);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.PartnerUsers.FindAsync(partnerUser.PartnerId, partnerUser.UserId).ConfigureAwait(false);
        }

        public IQueryable<PartnerUser> GetPartnerUsers(CancellationToken cancellationToken = default)
        {
            return mobDbContext.PartnerUsers.AsQueryable();
        }

        // Update the records of a particular Partner User
        public async Task<PartnerUser> UpdatePartnerUser(PartnerUser partnerUser)
        {
            mobDbContext.Entry(partnerUser).State = EntityState.Modified;
            partnerUser.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.PartnerUsers.FindAsync(partnerUser.PartnerId, partnerUser.UserId).ConfigureAwait(false);
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
