namespace TrashMob.Shared.Managers.Communities
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
    /// Manager for community page operations.
    /// Communities are partners with enabled home pages.
    /// </summary>
    public class CommunityManager : ICommunityManager
    {
        private readonly IKeyedRepository<Partner> partnerRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunityManager"/> class.
        /// </summary>
        /// <param name="partnerRepository">The partner repository.</param>
        public CommunityManager(IKeyedRepository<Partner> partnerRepository)
        {
            this.partnerRepository = partnerRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Partner>> GetEnabledCommunitiesAsync(
            double? latitude = null,
            double? longitude = null,
            double? radiusMiles = null,
            CancellationToken cancellationToken = default)
        {
            var now = DateTimeOffset.UtcNow;

            var communities = await partnerRepository.Get()
                .Where(p => p.HomePageEnabled
                    && (p.HomePageStartDate == null || p.HomePageStartDate <= now)
                    && (p.HomePageEndDate == null || p.HomePageEndDate >= now)
                    && p.PartnerStatusId == (int)PartnerStatusEnum.Active)
                .ToListAsync(cancellationToken);

            if (latitude.HasValue && longitude.HasValue && radiusMiles.HasValue)
            {
                communities = communities
                    .Where(c => c.Latitude.HasValue && c.Longitude.HasValue &&
                                CalculateDistance(latitude.Value, longitude.Value, c.Latitude.Value, c.Longitude.Value) <= radiusMiles.Value)
                    .ToList();
            }

            return communities;
        }

        /// <inheritdoc />
        public async Task<Partner> GetBySlugAsync(string slug, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = slug.Trim().ToLowerInvariant();
            var now = DateTimeOffset.UtcNow;

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            return await partnerRepository.Get()
                .FirstOrDefaultAsync(p => p.Slug.ToLower() == normalizedSlug
                    && p.HomePageEnabled
                    && (p.HomePageStartDate == null || p.HomePageStartDate <= now)
                    && (p.HomePageEndDate == null || p.HomePageEndDate >= now)
                    && p.PartnerStatusId == (int)PartnerStatusEnum.Active,
                    cancellationToken);
#pragma warning restore CA1862
        }

        /// <inheritdoc />
        public async Task<bool> IsSlugAvailableAsync(string slug, Guid? excludePartnerId = null, CancellationToken cancellationToken = default)
        {
            var normalizedSlug = slug.Trim().ToLowerInvariant();

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            var query = partnerRepository.Get()
                .Where(p => p.Slug.ToLower() == normalizedSlug);
#pragma warning restore CA1862

            if (excludePartnerId.HasValue)
            {
                query = query.Where(p => p.Id != excludePartnerId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        /// <summary>
        /// Calculates the distance between two geographic coordinates using the Haversine formula.
        /// </summary>
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 3959; // Earth's radius in miles

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
