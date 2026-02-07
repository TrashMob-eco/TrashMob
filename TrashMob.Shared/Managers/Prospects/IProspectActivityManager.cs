namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IProspectActivityManager : IKeyedManager<ProspectActivity>
    {
        Task<IEnumerable<ProspectActivity>> GetByProspectIdAsync(Guid prospectId, CancellationToken cancellationToken = default);
    }
}
