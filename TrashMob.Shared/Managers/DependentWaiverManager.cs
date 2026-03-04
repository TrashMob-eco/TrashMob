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
    using TrashMob.Shared.Poco;

    public class DependentWaiverManager(
        IKeyedRepository<DependentWaiver> repository,
        IKeyedRepository<Dependent> dependentRepository,
        IKeyedRepository<WaiverVersion> waiverVersionRepository)
        : KeyedManager<DependentWaiver>(repository), IDependentWaiverManager
    {
        public async Task<ServiceResult<DependentWaiver>> SignWaiverAsync(
            Guid dependentId,
            Guid waiverVersionId,
            string typedLegalName,
            string ipAddress,
            string userAgent,
            Guid signerUserId,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(typedLegalName))
            {
                return ServiceResult<DependentWaiver>.Failure("Typed legal name is required.");
            }

            var dependent = await dependentRepository.GetAsync(dependentId, cancellationToken);
            if (dependent == null)
            {
                return ServiceResult<DependentWaiver>.Failure("Dependent not found.");
            }

            if (dependent.ParentUserId != signerUserId)
            {
                return ServiceResult<DependentWaiver>.Failure("Only the parent/guardian can sign a waiver for this dependent.");
            }

            if (!dependent.IsActive)
            {
                return ServiceResult<DependentWaiver>.Failure("Cannot sign a waiver for an inactive dependent.");
            }

            var waiverVersion = await waiverVersionRepository.GetAsync(waiverVersionId, cancellationToken);
            if (waiverVersion == null || !waiverVersion.IsActive)
            {
                return ServiceResult<DependentWaiver>.Failure("Waiver version not found or is no longer active.");
            }

            var now = DateTimeOffset.UtcNow;
            var expiryDate = new DateTimeOffset(now.Year, 12, 31, 23, 59, 59, TimeSpan.Zero);

            var dependentWaiver = new DependentWaiver
            {
                Id = Guid.NewGuid(),
                DependentId = dependentId,
                WaiverVersionId = waiverVersionId,
                SignedByUserId = signerUserId,
                TypedLegalName = typedLegalName,
                WaiverTextSnapshot = waiverVersion.WaiverText,
                AcceptedDate = now,
                ExpiryDate = expiryDate,
                IPAddress = ipAddress,
                UserAgent = userAgent,
            };

            var created = await AddAsync(dependentWaiver, signerUserId, cancellationToken);
            return ServiceResult<DependentWaiver>.Success(created);
        }

        public async Task<bool> HasValidWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await Repo.Get(dw => dw.DependentId == dependentId && dw.ExpiryDate >= now)
                .AnyAsync(cancellationToken);
        }

        public async Task<DependentWaiver> GetCurrentWaiverAsync(Guid dependentId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;
            return await Repo.Get(dw => dw.DependentId == dependentId && dw.ExpiryDate >= now)
                .OrderByDescending(dw => dw.AcceptedDate)
                .FirstOrDefaultAsync(cancellationToken);
        }

        public async Task<IEnumerable<DependentWaiver>> GetByDependentIdAsync(Guid dependentId, CancellationToken cancellationToken = default)
        {
            return await Repo.Get(dw => dw.DependentId == dependentId)
                .OrderByDescending(dw => dw.AcceptedDate)
                .ToListAsync(cancellationToken);
        }
    }
}
