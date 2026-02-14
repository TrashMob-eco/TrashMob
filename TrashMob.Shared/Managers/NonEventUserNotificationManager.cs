namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages non-event user notifications such as profile reminders and general announcements.
    /// </summary>
    public class NonEventUserNotificationManager : KeyedManager<NonEventUserNotification>,
        INonEventUserNotificationManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonEventUserNotificationManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for non-event user notification data access.</param>
        public NonEventUserNotificationManager(IKeyedRepository<NonEventUserNotification> repository)
            : base(repository)
        {
        }

        /// <inheritdoc />
        public async Task<IEnumerable<NonEventUserNotification>> GetByUserIdAsync(Guid userId,
            int userNotificationTypeId, CancellationToken cancellationToken = default)
        {
            return await Repository.Get(n => n.UserId == userId && n.UserNotificationTypeId == userNotificationTypeId)
                .ToListAsync(cancellationToken);
        }
    }
}