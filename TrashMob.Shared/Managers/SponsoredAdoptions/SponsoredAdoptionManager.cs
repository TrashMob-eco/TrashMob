namespace TrashMob.Shared.Managers.SponsoredAdoptions
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

    /// <summary>
    /// Manager for sponsored adoption operations within a community.
    /// </summary>
    public class SponsoredAdoptionManager(IKeyedRepository<SponsoredAdoption> repository)
        : KeyedManager<SponsoredAdoption>(repository), ISponsoredAdoptionManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<SponsoredAdoption>> GetByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(sa => sa.AdoptableArea)
                .Include(sa => sa.Sponsor)
                .Include(sa => sa.ProfessionalCompany)
                .Where(sa => sa.AdoptableArea.PartnerId == partnerId)
                .OrderByDescending(sa => sa.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SponsoredAdoption>> GetBySponsorIdAsync(
            Guid sponsorId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(sa => sa.AdoptableArea)
                .Include(sa => sa.ProfessionalCompany)
                .Where(sa => sa.SponsorId == sponsorId)
                .OrderByDescending(sa => sa.StartDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<SponsoredAdoption>> GetByCompanyIdAsync(
            Guid companyId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(sa => sa.AdoptableArea)
                .Include(sa => sa.Sponsor)
                .Where(sa => sa.ProfessionalCompanyId == companyId && sa.Status == SponsoredAdoptionStatus.Active)
                .OrderBy(sa => sa.AdoptableArea.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<SponsoredAdoptionComplianceStats> GetComplianceByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            var adoptions = await Repo.Get()
                .Include(sa => sa.AdoptableArea)
                .Include(sa => sa.CleanupLogs)
                .Where(sa => sa.AdoptableArea.PartnerId == partnerId)
                .ToListAsync(cancellationToken);

            var now = DateTimeOffset.UtcNow;
            var stats = new SponsoredAdoptionComplianceStats
            {
                TotalSponsoredAdoptions = adoptions.Count,
                ActiveAdoptions = adoptions.Count(a => a.Status == SponsoredAdoptionStatus.Active),
                ExpiredAdoptions = adoptions.Count(a => a.Status == SponsoredAdoptionStatus.Expired),
                TerminatedAdoptions = adoptions.Count(a => a.Status == "Terminated"),
            };

            var activeAdoptions = adoptions.Where(a => a.Status == SponsoredAdoptionStatus.Active).ToList();
            foreach (var adoption in activeAdoptions)
            {
                var lastCleanup = adoption.CleanupLogs
                    .OrderByDescending(l => l.CleanupDate)
                    .FirstOrDefault();

                if (lastCleanup != null &&
                    (now - lastCleanup.CleanupDate).TotalDays <= adoption.CleanupFrequencyDays)
                {
                    stats.AdoptionsOnSchedule++;
                }
                else
                {
                    stats.AdoptionsOverdue++;
                }
            }

            var allLogs = adoptions.SelectMany(a => a.CleanupLogs).ToList();
            stats.TotalCleanupLogs = allLogs.Count;
            stats.TotalWeightCollectedPounds = allLogs.Sum(l => l.WeightInPounds ?? 0);
            stats.TotalBagsCollected = allLogs.Sum(l => l.BagsCollected);

            return stats;
        }
    }
}
