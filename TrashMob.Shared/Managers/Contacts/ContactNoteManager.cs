namespace TrashMob.Shared.Managers.Contacts
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ContactNoteManager(IKeyedRepository<ContactNote> repository)
        : KeyedManager<ContactNote>(repository), IContactNoteManager
    {
        public async Task<IEnumerable<ContactNote>> GetByContactIdAsync(Guid contactId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(n => n.ContactId == contactId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
