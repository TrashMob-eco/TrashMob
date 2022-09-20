namespace TrashMob.Shared.Persistence
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class NonEventUserNotificationRepository : INonEventUserNotificationRepository
    {
        private readonly MobDbContext mobDbContext;

        public NonEventUserNotificationRepository(MobDbContext mobDbContext)
        {
            this.mobDbContext = mobDbContext;
        }

        public async Task<IEnumerable<NonEventUserNotification>> GetNonEventUserNotifications(Guid userId, CancellationToken cancellationToken = default)
        {
            return await mobDbContext.NonEventUserNotifications.Where(un => un.UserId == userId)
                .AsNoTracking()
                .ToListAsync(cancellationToken: cancellationToken).ConfigureAwait(false);
        }

        public async Task<NonEventUserNotification> AddNonEventUserNotification(NonEventUserNotification nonEventUserNotification)
        {
            nonEventUserNotification.Id = Guid.NewGuid();
            nonEventUserNotification.SentDate = DateTimeOffset.UtcNow;
            mobDbContext.NonEventUserNotifications.Add(nonEventUserNotification);
            await mobDbContext.SaveChangesAsync().ConfigureAwait(false);
            return await mobDbContext.NonEventUserNotifications.FindAsync(nonEventUserNotification.Id).ConfigureAwait(false);
        }
    }
}
