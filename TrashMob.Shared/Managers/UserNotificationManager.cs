namespace TrashMob.Shared.Managers.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class UserNotificationManager : KeyedManager<UserNotification>, IKeyedManager<UserNotification>
    {
        public UserNotificationManager(IKeyedRepository<UserNotification> repository) : base(repository)
        {
        }

        public override async Task<IEnumerable<UserNotification>> GetCollectionAsync(Guid parentId, Guid secondId, CancellationToken cancellationToken)
        {
            var result = await Repo.Get(un => un.UserId == parentId && un.EventId == secondId).ToListAsync(cancellationToken);

            return result;
        }
    }
}
