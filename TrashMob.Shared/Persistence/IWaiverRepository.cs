namespace TrashMob.Shared.Persistence
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;

    public interface IWaiverRepository
    {
        Task<IEnumerable<Waiver>> GetWaivers(CancellationToken cancellationToken = default);

        Task<Waiver> GetWaiver(Guid id, CancellationToken cancellationToken = default);

        Task<Waiver> AddWaiver(Waiver waiver);

        Task<Waiver> UpdateWaiver(Waiver waiver);
    }
}
