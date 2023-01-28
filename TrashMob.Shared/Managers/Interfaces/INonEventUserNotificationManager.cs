namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface INonEventUserNotificationManager : IKeyedManager<NonEventUserNotification>
    {
        Task<IEnumerable<NonEventUserNotification>> GetByUserIdAsync(Guid userId, int userNotificationTypeId, CancellationToken cancellationToken = default);
    }
}
