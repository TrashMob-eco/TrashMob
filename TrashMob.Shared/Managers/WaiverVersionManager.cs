namespace TrashMob.Shared.Managers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for waiver version operations and community waiver assignments.
    /// </summary>
    public class WaiverVersionManager(
        IKeyedRepository<WaiverVersion> repository,
        IBaseRepository<CommunityWaiver> communityWaiverRepository)
        : KeyedManager<WaiverVersion>(repository), IWaiverVersionManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .OrderByDescending(w => w.EffectiveDate)
                .ThenBy(w => w.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetActiveWaiversAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await Repo.Get(w =>
                    w.IsActive &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .ThenBy(w => w.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<WaiverVersion> GetCurrentGlobalWaiverAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await Repo.Get(w =>
                    w.IsActive &&
                    w.Scope == WaiverScope.Global &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetCommunityWaiversAsync(Guid communityId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            var communityWaiverIds = await communityWaiverRepository
                .Get(cw => cw.CommunityId == communityId)
                .Select(cw => cw.WaiverVersionId)
                .ToListAsync(cancellationToken);

            if (!communityWaiverIds.Any())
            {
                return Enumerable.Empty<WaiverVersion>();
            }

            return await Repo.Get(w =>
                    communityWaiverIds.Contains(w.Id) &&
                    w.IsActive &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<CommunityWaiver> AssignToCommunityAsync(Guid waiverId, Guid communityId, Guid userId, CancellationToken cancellationToken = default)
        {
            // Check if assignment already exists
            var existing = await communityWaiverRepository
                .Get(cw => cw.WaiverVersionId == waiverId && cw.CommunityId == communityId)
                .FirstOrDefaultAsync(cancellationToken);

            if (existing != null)
            {
                throw new InvalidOperationException("This waiver is already assigned to this community.");
            }

            var communityWaiver = new CommunityWaiver
            {
                Id = Guid.NewGuid(),
                WaiverVersionId = waiverId,
                CommunityId = communityId,
                IsRequired = true,
                CreatedByUserId = userId,
                LastUpdatedByUserId = userId,
                CreatedDate = DateTimeOffset.UtcNow,
                LastUpdatedDate = DateTimeOffset.UtcNow,
            };

            return await communityWaiverRepository.AddAsync(communityWaiver);
        }

        /// <inheritdoc />
        public async Task RemoveFromCommunityAsync(Guid waiverId, Guid communityId, CancellationToken cancellationToken = default)
        {
            var communityWaiver = await communityWaiverRepository
                .Get(cw => cw.WaiverVersionId == waiverId && cw.CommunityId == communityId)
                .FirstOrDefaultAsync(cancellationToken);

            if (communityWaiver != null)
            {
                await communityWaiverRepository.DeleteAsync(communityWaiver);
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<CommunityWaiver>> GetCommunityWaiverAssignmentsAsync(Guid communityId, CancellationToken cancellationToken = default)
        {
            return await communityWaiverRepository
                .Get(cw => cw.CommunityId == communityId)
                .Include(cw => cw.WaiverVersion)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task DeactivateAsync(Guid waiverId, Guid userId, CancellationToken cancellationToken = default)
        {
            var waiver = await Repo.GetAsync(waiverId, cancellationToken);
            if (waiver == null)
            {
                throw new InvalidOperationException("Waiver not found.");
            }

            waiver.IsActive = false;
            waiver.LastUpdatedByUserId = userId;
            waiver.LastUpdatedDate = DateTimeOffset.UtcNow;

            await Repository.UpdateAsync(waiver);
        }
    }
}
