namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserNotificationRepository : IUserNotificationRepository
    {
        private readonly MobDbContext mobDbContext;

        public UserNotificationRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<UserNotification>> GetUserNotifications(Guid userId, Guid eventId, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.UserNotifications.Where(un => un.EventId == eventId && un.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<UserNotification> AddUserNotification(UserNotification userNotification)
        {
            userNotification.Id = Guid.NewGuid();
            userNotification.SentDate = DateTimeOffset.UtcNow;
            mobDbContext.UserNotifications.Add(userNotification);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.UserNotifications.FindAsync(userNotification.Id).ConfigureAwait(false);
        }
    }
}
