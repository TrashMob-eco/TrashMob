namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface INonEventUserNotificationRepository
    {
        Task<IEnumerable<NonEventUserNotification>> GetNonEventUserNotifications(Guid userId, CancellationToken cancellationToken = default);

        Task<NonEventUserNotification> AddNonEventUserNotification(NonEventUserNotification nonEventUserNotification);
    }
}
