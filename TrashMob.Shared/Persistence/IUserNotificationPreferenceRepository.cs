namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IUserNotificationPreferenceRepository
    {
        Task<IEnumerable<UserNotificationPreference>> GetUserNotificationPreferences(Guid userId, CancellationToken cancellationToken = default);

        Task<UserNotificationPreference> AddUpdateUserNotificationPreference(UserNotificationPreference userNotificationPreference);
    }
}
