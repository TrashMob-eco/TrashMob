namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IDonationManager : IKeyedManager<Donation>
    {
        Task<IEnumerable<Donation>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    }
}
