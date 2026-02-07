namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ProspectActivityManager : KeyedManager<ProspectActivity>, IProspectActivityManager
    {
        public ProspectActivityManager(IKeyedRepository<ProspectActivity> repository)
            : base(repository)
        {
        }

        public async Task<IEnumerable<ProspectActivity>> GetByProspectIdAsync(Guid prospectId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.ProspectId == prospectId)
                .OrderByDescending(a => a.CreatedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
