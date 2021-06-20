namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IUserNotificationRepository
    {
        Task<IEnumerable<UserNotification>> GetUserNotifications(Guid userId, Guid eventId);

        Task<UserNotification> AddUserNotification(UserNotification userNotification);
    }
}
