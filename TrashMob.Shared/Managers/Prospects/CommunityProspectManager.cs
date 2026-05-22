namespace TrashMob.Shared.Managers.Prospects
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers;
    using TrashMob.Shared.Persistence.Interfaces;

    public class CommunityProspectManager(
        IKeyedRepository<CommunityProspect> repository,
        IKeyedRepository<ProspectContact> contactRepository)
        : KeyedManager<CommunityProspect>(repository), ICommunityProspectManager
    {
        public override Task<CommunityProspect> GetAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return Repo.Get()
                .Include(p => p.Contacts)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public override async Task<IEnumerable<CommunityProspect>> GetAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(p => p.Contacts)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CommunityProspect>> GetByPipelineStageAsync(int stage, CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(p => p.Contacts)
                .Where(p => p.PipelineStage == stage)
                .OrderByDescending(p => p.FitScore)
                .ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<CommunityProspect>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            var term = searchTerm.Trim().ToLowerInvariant();
            var like = $"%{term}%";

            return await Repo.Get()
                .Include(p => p.Contacts)
                .Where(p =>
                    EF.Functions.Like(p.Name, like) ||
                    EF.Functions.Like(p.City, like) ||
                    EF.Functions.Like(p.Region, like) ||
                    p.Contacts.Any(c =>
                        EF.Functions.Like(c.Name, like) ||
                        EF.Functions.Like(c.Email, like)))
                .OrderByDescending(p => p.FitScore)
                .ToListAsync(cancellationToken);
        }

        public async Task<CommunityProspect> UpdatePipelineStageAsync(Guid id, int newStage, Guid userId, CancellationToken cancellationToken = default)
        {
            var prospect = await Repo.GetAsync(id, cancellationToken);

            if (prospect is null)
            {
                return null;
            }

            prospect.PipelineStage = newStage;
            prospect.LastUpdatedByUserId = userId;
            prospect.LastUpdatedDate = DateTimeOffset.UtcNow;

            return await Repo.UpdateAsync(prospect);
        }

        public async Task<ProspectContact> UpsertPrimaryContactAsync(
            Guid prospectId,
            string name,
            string email,
            string title,
            string phone,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Find the existing primary contact, if any.
            var primary = await contactRepository.Get()
                .Where(c => c.ProspectId == prospectId && c.IsPrimary)
                .FirstOrDefaultAsync(cancellationToken);

            var now = DateTimeOffset.UtcNow;

            if (primary == null)
            {
                primary = new ProspectContact
                {
                    Id = Guid.NewGuid(),
                    ProspectId = prospectId,
                    Name = string.IsNullOrWhiteSpace(name) ? "(unknown)" : name,
                    Email = email,
                    Title = title,
                    Phone = phone,
                    ContactStatus = (int)ProspectContactStatus.Active,
                    IsPrimary = true,
                    CreatedByUserId = userId,
                    CreatedDate = now,
                    LastUpdatedByUserId = userId,
                    LastUpdatedDate = now,
                };

                return await contactRepository.AddAsync(primary);
            }

            // Update fields that were provided (treat empty string as "no change" too — callers
            // who want to clear a field should use the dedicated contacts API in Phase 2).
            if (!string.IsNullOrWhiteSpace(name))
            {
                primary.Name = name;
            }

            if (!string.IsNullOrWhiteSpace(email))
            {
                primary.Email = email;
            }

            if (!string.IsNullOrWhiteSpace(title))
            {
                primary.Title = title;
            }

            if (!string.IsNullOrWhiteSpace(phone))
            {
                primary.Phone = phone;
            }

            primary.LastUpdatedByUserId = userId;
            primary.LastUpdatedDate = now;

            return await contactRepository.UpdateAsync(primary);
        }
    }
}
