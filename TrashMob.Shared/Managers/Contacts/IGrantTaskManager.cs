namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IGrantTaskManager : IKeyedManager<GrantTask>
    {
        Task<IEnumerable<GrantTask>> GetByGrantIdAsync(Guid grantId, CancellationToken cancellationToken = default);
    }
}
