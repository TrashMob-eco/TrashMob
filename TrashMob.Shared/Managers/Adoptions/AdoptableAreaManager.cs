namespace TrashMob.Shared.Managers.Adoptions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using TrashMob.Models;
    using TrashMob.Models.Poco;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    /// <summary>
    /// Manager for adoptable area operations.
    /// </summary>
    public class AdoptableAreaManager(IKeyedRepository<AdoptableArea> repository, ILogger<AdoptableAreaManager> logger)
        : KeyedManager<AdoptableArea>(repository), IAdoptableAreaManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<AdoptableArea>> GetByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.PartnerId == partnerId && a.IsActive)
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<AdoptableArea>> GetAvailableByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(a => a.PartnerId == partnerId && a.IsActive && a.Status == "Available")
                .OrderBy(a => a.Name)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsNameAvailableAsync(
            Guid partnerId,
            string name,
            Guid? excludeAreaId = null,
            CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLowerInvariant();

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            var query = Repo.Get()
                .Where(a => a.PartnerId == partnerId && a.Name.ToLower() == normalizedName);
#pragma warning restore CA1862

            if (excludeAreaId.HasValue)
            {
                query = query.Where(a => a.Id != excludeAreaId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<AreaBulkImportResult> BulkCreateAsync(
            Guid partnerId,
            Guid userId,
            IEnumerable<AdoptableArea> areas,
            CancellationToken cancellationToken = default)
        {
            var result = new AreaBulkImportResult();

            // Load existing area names for dedup (single query)
            var existingNames = new HashSet<string>(
                await Repo.Get()
                    .Where(a => a.PartnerId == partnerId && a.IsActive)
                    .Select(a => a.Name.ToLower())
                    .ToListAsync(cancellationToken),
                StringComparer.OrdinalIgnoreCase);

            // Track names added in this batch to catch intra-batch duplicates
            var batchNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            var index = 0;
            foreach (var area in areas)
            {
                result.TotalProcessed++;
                var featureName = area.Name ?? $"Feature {index}";

                if (string.IsNullOrWhiteSpace(area.Name))
                {
                    result.ErrorCount++;
                    result.Errors.Add(new AreaImportError
                    {
                        FeatureIndex = index,
                        FeatureName = featureName,
                        Message = "Name is required.",
                    });
                    index++;
                    continue;
                }

                var normalizedName = area.Name.Trim().ToLowerInvariant();

                // Check for duplicates against existing DB names and batch names
                if (existingNames.Contains(normalizedName) || batchNames.Contains(normalizedName))
                {
                    result.SkippedDuplicateCount++;
                    index++;
                    continue;
                }

                try
                {
                    // Set required fields
                    area.Id = Guid.NewGuid();
                    area.PartnerId = partnerId;
                    area.IsActive = true;
                    area.CreatedByUserId = userId;
                    area.CreatedDate = DateTimeOffset.UtcNow;
                    area.LastUpdatedByUserId = userId;
                    area.LastUpdatedDate = DateTimeOffset.UtcNow;

                    await Repo.AddAsync(area);
                    batchNames.Add(normalizedName);
                    result.CreatedCount++;
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Failed to import area '{AreaName}' at index {Index}", featureName, index);
                    result.ErrorCount++;
                    result.Errors.Add(new AreaImportError
                    {
                        FeatureIndex = index,
                        FeatureName = featureName,
                        Message = $"Import failed: {ex.Message}",
                    });
                }

                index++;
            }

            return result;
        }

        /// <inheritdoc />
        public async Task<int> ClearAllByPartnerAsync(
            Guid partnerId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            var areas = await Repo.Get(a => a.PartnerId == partnerId && a.IsActive, withNoTracking: false)
                .ToListAsync(cancellationToken);

            var now = DateTimeOffset.UtcNow;

            foreach (var area in areas)
            {
                area.IsActive = false;
                area.LastUpdatedByUserId = userId;
                area.LastUpdatedDate = now;
                await Repo.UpdateAsync(area);
            }

            return areas.Count;
        }
    }
}
