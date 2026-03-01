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

    public class PledgeManager(IKeyedRepository<Pledge> repository)
        : KeyedManager<Pledge>(repository), IPledgeManager
    {
        public async Task<IEnumerable<Pledge>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(p => p.ContactId == contactId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
