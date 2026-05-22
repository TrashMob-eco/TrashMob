namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Persistence.Interfaces;

    public class ProspectContactManager(
        IKeyedRepository<ProspectContact> repository,
        IKeyedRepository<ProspectActivity> activityRepository,
        IKeyedRepository<ProspectOutreachEmail> outreachEmailRepository)
        : KeyedManager<ProspectContact>(repository), IProspectContactManager
    {
        public async Task<IEnumerable<ProspectContact>> GetByProspectAsync(Guid prospectId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(c => c.ProspectId == prospectId)
                .OrderByDescending(c => c.IsPrimary)
                .ThenBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<ProspectContact> SetPrimaryAsync(Guid contactId, Guid userId, CancellationToken cancellationToken = default)
        {
            var target = await Repo.GetAsync(contactId, cancellationToken);
            if (target is null)
            {
                return null;
            }

            var siblings = await Repo.Get()
                .Where(c => c.ProspectId == target.ProspectId && c.Id != target.Id && c.IsPrimary)
                .ToListAsync(cancellationToken);

            var now = DateTimeOffset.UtcNow;

            // Clear IsPrimary on existing primaries (if any) and persist.
            foreach (var sibling in siblings)
            {
                sibling.IsPrimary = false;
                sibling.LastUpdatedByUserId = userId;
                sibling.LastUpdatedDate = now;
                await Repo.UpdateAsync(sibling);
            }

            target.IsPrimary = true;
            target.LastUpdatedByUserId = userId;
            target.LastUpdatedDate = now;

            return await Repo.UpdateAsync(target);
        }

        public async Task<ProspectContact> UpdateStatusAsync(Guid contactId, int newStatus, Guid userId, CancellationToken cancellationToken = default)
        {
            var contact = await Repo.GetAsync(contactId, cancellationToken);
            if (contact is null)
            {
                return null;
            }

            contact.ContactStatus = newStatus;
            contact.LastUpdatedByUserId = userId;
            contact.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repo.UpdateAsync(contact);
        }

        public async Task<bool> HasReferencesAsync(Guid contactId, CancellationToken cancellationToken = default)
        {
            var activityRefs = await activityRepository.Get()
                .AnyAsync(a => a.ProspectContactId == contactId, cancellationToken);

            if (activityRefs)
            {
                return true;
            }

            return await outreachEmailRepository.Get()
                .AnyAsync(e => e.ProspectContactId == contactId, cancellationToken);
        }
    }
}
