namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class GrantTaskManager(IKeyedRepository<GrantTask> repository)
        : KeyedManager<GrantTask>(repository), IGrantTaskManager
    {
        public async Task<IEnumerable<GrantTask>> GetByGrantIdAsync(Guid grantId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(t => t.GrantId == grantId)
                .OrderBy(t => t.SortOrder)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
