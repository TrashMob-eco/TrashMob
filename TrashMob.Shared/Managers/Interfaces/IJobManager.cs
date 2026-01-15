using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TrashMob.Models;

namespace TrashMob.Shared.Managers.Interfaces;

public interface IJobManager : IKeyedManager<JobOpportunity>
{
    Task<IEnumerable<JobOpportunity>> GetJobsAsync(bool isActive, CancellationToken cancellationToken = default);
}