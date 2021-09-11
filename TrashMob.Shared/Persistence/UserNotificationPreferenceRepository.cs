namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public class UserNotificationPreferenceRepository : IUserNotificationPreferenceRepository
    {
        private readonly MobDbContext mobDbContext;

        public UserNotificationPreferenceRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<UserNotificationPreference>> GetUserNotificationPreferences(Guid userId)
        {
            return await mobDbContext.UserNotificationPreferences.Where(unp => unp.UserId == userId)
                .AsNoTracking()
                .ToListAsync().ConfigureAwait(false);
        }

        public async Task<UserNotificationPreference> AddUpdateUserNotificationPreference(UserNotificationPreference userNotificationPreference)
        {
            var unp = mobDbContext.UserNotificationPreferences.Where(u => u.UserId == userNotificationPreference.UserId && u.UserNotificationTypeId == userNotificationPreference.UserNotificationTypeId).FirstOrDefault();

            if (unp == null)
            {
                userNotificationPreference.Id = Guid.NewGuid();
                userNotificationPreference.LastUpdatedDate = DateTimeOffset.UtcNow;
                mobDbContext.UserNotificationPreferences.Add(userNotificationPreference);
            }
            else
            {
                unp.IsOptedOut = userNotificationPreference.IsOptedOut;
                unp.LastUpdatedDate = DateTimeOffset.UtcNow;
            }

            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);

            return await mobDbContext.UserNotificationPreferences.FindAsync(userNotificationPreference.Id).ConfigureAwait(false);
        }
    }
}
