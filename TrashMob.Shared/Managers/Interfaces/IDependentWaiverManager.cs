namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface IDependentWaiverManager : IKeyedManager<DependentWaiver>
    {
        Task<ServiceResult<DependentWaiver>> SignWaiverAsync(
            Guid dependentId,
            Guid waiverVersionId,
            string typedLegalName,
            string ipAddress,
            string userAgent,
            Guid signerUserId,
            CancellationToken cancellationToken = default);

        Task<bool> HasValidWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default);

        Task<DependentWaiver> GetCurrentWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default);

        Task<IEnumerable<DependentWaiver>> GetByDependentIdAsync(Guid dependentId, CancellationToken cancellationToken = default);
    }
}
