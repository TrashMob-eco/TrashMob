namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    public interface IDependentManager : IKeyedManager<Dependent>
    {
        Task<IEnumerable<Dependent>> GetByParentUserIdAsync(Guid parentUserId, CancellationToken cancellationToken = default);

        Task<int> SoftDeleteAsync(Guid dependentId, Guid userId, CancellationToken cancellationToken = default);
    }
}
