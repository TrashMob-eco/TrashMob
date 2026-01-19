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
    public class UserNotificationManager : KeyedManager<UserNotification>, IKeyedManager<UserNotification>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserNotificationManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for user notification data access.</param>
        public UserNotificationManager(IKeyedRepository<UserNotification> repository) : base(repository)
        {
        }

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