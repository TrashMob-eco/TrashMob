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

    /// <summary>
    /// Manager for sponsor operations within a community's sponsored adoption program.
    /// </summary>
    public class SponsorManager(IKeyedRepository<Sponsor> repository)
        : KeyedManager<Sponsor>(repository), ISponsorManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<Sponsor>> GetByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(s => s.PartnerId == partnerId && s.IsActive)
                .OrderBy(s => s.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
