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
        IKeyedRepository<WaiverVersion> waiverVersionRepository,
        IBaseRepository<CommunityWaiver> communityWaiverRepository,
        IBaseRepository<EventPartnerLocationService> eventPartnerLocationServiceRepository)
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

        /// <inheritdoc />
        public async Task<IEnumerable<WaiverVersion>> GetRequiredWaiversForDependentEventAsync(
            Guid dependentId, Guid eventId, CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            // Get the dependent's current valid waivers
            var signedWaiverVersionIds = await Repo.Get(dw =>
                    dw.DependentId == dependentId && dw.ExpiryDate >= now)
                .Select(dw => dw.WaiverVersionId)
                .ToListAsync(cancellationToken);

            var signedSet = signedWaiverVersionIds.ToHashSet();

            // Get the current global waiver
            var globalWaiver = await waiverVersionRepository
                .Get(w =>
                    w.IsActive &&
                    w.Scope == WaiverScope.Global &&
                    w.EffectiveDate <= now &&
                    (w.ExpiryDate == null || w.ExpiryDate > now))
                .OrderByDescending(w => w.EffectiveDate)
                .FirstOrDefaultAsync(cancellationToken);

            // Get partner IDs for the event
            var partnerIds = await eventPartnerLocationServiceRepository
                .Get(epls => epls.EventId == eventId)
                .Include(epls => epls.PartnerLocation)
                .Select(epls => epls.PartnerLocation.PartnerId)
                .Distinct()
                .ToListAsync(cancellationToken);

            // Get required community waiver versions
            var communityWaiverVersionIds = await communityWaiverRepository
                .Get(cw => partnerIds.Contains(cw.CommunityId) && cw.IsRequired)
                .Select(cw => cw.WaiverVersionId)
                .ToListAsync(cancellationToken);

            var communityWaivers = communityWaiverVersionIds.Count != 0
                ? await waiverVersionRepository.Get(w =>
                        communityWaiverVersionIds.Contains(w.Id) &&
                        w.IsActive &&
                        w.EffectiveDate <= now &&
                        (w.ExpiryDate == null || w.ExpiryDate > now))
                    .ToListAsync(cancellationToken)
                : [];

            // Combine and filter out already signed
            List<WaiverVersion> requiredWaivers = [];

            if (globalWaiver is not null && !signedSet.Contains(globalWaiver.Id))
            {
                requiredWaivers.Add(globalWaiver);
            }

            requiredWaivers.AddRange(communityWaivers.Where(w => !signedSet.Contains(w.Id)));

            return requiredWaivers;
        }

        /// <inheritdoc />
        public async Task<bool> HasValidWaiversForEventAsync(
            Guid dependentId, Guid eventId, CancellationToken cancellationToken = default)
        {
            var required = await GetRequiredWaiversForDependentEventAsync(dependentId, eventId, cancellationToken);
            return !required.Any();
        }

        public async Task<(int Total, int Valid, int Expired, Dictionary<string, int> RelationshipBreakdown)> GetComplianceStatsAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var total = await Repo.Get().CountAsync(cancellationToken);
            var valid = await Repo.Get(dw => dw.ExpiryDate >= now).CountAsync(cancellationToken);
            var expired = total - valid;

            var relationships = await Repo.Get(dw => dw.ExpiryDate >= now)
                .Include(dw => dw.Dependent)
                .GroupBy(dw => dw.Dependent.Relationship)
                .Select(g => new { Relationship = g.Key ?? "Unknown", Count = g.Count() })
                .ToListAsync(cancellationToken);

            var breakdown = relationships.ToDictionary(r => r.Relationship, r => r.Count);

            return (total, valid, expired, breakdown);
        }
    }
}
