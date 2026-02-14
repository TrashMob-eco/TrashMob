namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manages event-specific user notification records for tracking which notifications have been sent.
    /// </summary>
    public class UserNotificationManager(IKeyedRepository<UserNotification> repository)
        : KeyedManager<UserNotification>(repository), IKeyedManager<UserNotification>
    {

        /// <inheritdoc />
        public override async Task<IEnumerable<UserNotification>> GetCollectionAsync(Guid parentId, Guid secondId,
            CancellationToken cancellationToken)
        {
            var result = await Repo.Get(un => un.UserId == parentId && un.EventId == secondId)
                .ToListAsync(cancellationToken);

            return result;
        }
    }
}