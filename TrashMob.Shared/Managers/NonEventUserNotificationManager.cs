
namespace TrashMob.Shared.Managers
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    using System;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Models;
    using System.Threading;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;
    using System.Linq;

    public class NonEventUserNotificationManager : KeyedManager<NonEventUserNotification>, INonEventUserNotificationManager
    {
        public NonEventUserNotificationManager(IKeyedRepository<NonEventUserNotification> repository) 
            : base(repository)
        { 
        }

        public async Task<IEnumerable<NonEventUserNotification>> GetByUserIdAsync(Guid userId, int userNotificationTypeId, CancellationToken cancellationToken = default)
        {
            return (await Repository.Get(n => n.UserId == userId && n.UserNotificationTypeId == userNotificationTypeId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
