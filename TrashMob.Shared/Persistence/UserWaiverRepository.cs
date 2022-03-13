namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class UserWaiverRepository : IUserWaiverRepository
    {
        private readonly MobDbContext mobDbContext;

        public UserWaiverRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<UserWaiver> AddUserWaiver(UserWaiver userWaiver)
        {
            userWaiver.CreatedDate = DateTimeOffset.UtcNow;
            userWaiver.LastUpdatedByUserId = userWaiver.CreatedByUserId;
            userWaiver.LastUpdatedDate = DateTimeOffset.UtcNow;

            mobDbContext.UserWaivers.Add(userWaiver);

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.UserWaivers.FindAsync(userWaiver.UserId, userWaiver.WaiverId).ConfigureAwait(false);
        }

        public IQueryable<UserWaiver> GetUserWaivers(CancellationToken cancellationToken = default)
        {
            return mobDbContext.UserWaivers.AsQueryable();
        }

        // Update the records of a particular Partner User
        public async Task<UserWaiver> UpdateUserWaiver(UserWaiver userWaiver)
        {
            mobDbContext.Entry(userWaiver).State = EntityState.Modified;
            userWaiver.LastUpdatedDate = DateTimeOffset.UtcNow;
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.UserWaivers.FindAsync(userWaiver.UserId, userWaiver.WaiverId).ConfigureAwait(false);
        }

        public async Task<int> DeleteUserWaiver(Guid partnerId, Guid userId)
        {
            var partnerUser = await mobDbContext.UserWaivers.FindAsync(partnerId, userId).ConfigureAwait(false);
            mobDbContext.UserWaivers.Remove(partnerUser);

            // Save the changes
            return await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
        }
    }
}
