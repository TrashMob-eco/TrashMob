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

    public class ContactManager(IKeyedRepository<Contact> repository)
        : KeyedManager<Contact>(repository), IContactManager
    {
        public async Task<IEnumerable<Contact>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.Trim();
            return await Repo.Get()
                .Where(c =>
                    EF.Functions.Like(c.FirstName, $"%{term}%") ||
                    EF.Functions.Like(c.LastName, $"%{term}%") ||
                    EF.Functions.Like(c.Email, $"%{term}%") ||
                    EF.Functions.Like(c.OrganizationName, $"%{term}%"))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contact>> GetByContactTypeAsync(int contactType, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(c => c.ContactType == contactType)
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        public async Task<IEnumerable<Contact>> GetByTagAsync(Guid tagId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(c => c.ContactContactTags.Any(ct => ct.ContactTagId == tagId))
                .OrderBy(c => c.LastName)
                .ThenBy(c => c.FirstName)
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
