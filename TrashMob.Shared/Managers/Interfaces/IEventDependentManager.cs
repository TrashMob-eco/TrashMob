namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    public interface IEventDependentManager : IKeyedManager<EventDependent>
    {
        Task<ServiceResult<IEnumerable<EventDependent>>> RegisterDependentsAsync(
            Guid eventId,
            IEnumerable<Guid> dependentIds,
            Guid parentUserId,
            CancellationToken cancellationToken = default);

        Task<IEnumerable<EventDependent>> GetByEventIdAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<int> GetDependentCountAsync(Guid eventId, CancellationToken cancellationToken = default);

        Task<int> UnregisterDependentAsync(Guid eventId, Guid dependentId, Guid userId, CancellationToken cancellationToken = default);
    }
}
