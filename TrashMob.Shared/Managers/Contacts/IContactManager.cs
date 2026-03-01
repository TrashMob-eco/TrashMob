namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IContactManager : IKeyedManager<Contact>
    {
        Task<IEnumerable<Contact>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);

        Task<IEnumerable<Contact>> GetByContactTypeAsync(int contactType, CancellationToken cancellationToken = default);

        Task<IEnumerable<Contact>> GetByTagAsync(Guid tagId, CancellationToken cancellationToken = default);
    }
}
