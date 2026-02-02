namespace TrashMob.Shared.Managers.Adoptions
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
    /// Manager for adoptable area operations.
    /// </summary>
    public class AdoptableAreaManager : KeyedManager<AdoptableArea>, IAdoptableAreaManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AdoptableAreaManager"/> class.
        /// </summary>
        /// <param name="repository">The adoptable area repository.</param>
        public AdoptableAreaManager(IKeyedRepository<AdoptableArea> repository)
            : base(repository)
        {
        }

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
    }
}
