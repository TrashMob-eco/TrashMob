using System.Collections.Generic;
using System.Data.Entity;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;
using TrashMob.Shared.Persistence.Interfaces;

namespace TrashMob.Shared.Managers;

public class JobManager(
    IKeyedRepository<JobOpportunity> repository)
    : KeyedManager<JobOpportunity>(repository), IJobManager
{
    public async Task<IEnumerable<JobOpportunity>> GetJobsAsync(bool isActive,
        CancellationToken cancellationToken = default)
    {
        return await Repo.Get(e => e.IsActive == isActive)
            .ToListAsync(cancellationToken).ConfigureAwait(false);
    }
}