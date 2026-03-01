namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public interface IContactNoteManager : IKeyedManager<ContactNote>
    {
        Task<IEnumerable<ContactNote>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default);
    }
}
