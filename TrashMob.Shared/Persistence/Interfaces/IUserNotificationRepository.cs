namespace TrashMob.Shared.Persistence.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IUserNotificationRepository
    {
        Task<IEnumerable<UserNotification>> GetUserNotifications(Guid userId, Guid eventId, CancellationToken cancellationToken = default);

        Task<UserNotification> AddUserNotification(UserNotification userNotification);
    }
}
