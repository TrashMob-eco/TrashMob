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

    public class NonEventUserNotificationManager : KeyedManager<NonEventUserNotification>,
        INonEventUserNotificationManager
    {
        public NonEventUserNotificationManager(IKeyedRepository<NonEventUserNotification> repository)
            : base(repository)
        {
        }

        public async Task<IEnumerable<NonEventUserNotification>> GetByUserIdAsync(Guid userId,
            int userNotificationTypeId, CancellationToken cancellationToken = default)
        {
            return (await Repository.Get(n => n.UserId == userId && n.UserNotificationTypeId == userNotificationTypeId)
                .ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}