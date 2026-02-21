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
    /// Manager for professional cleanup company operations within a community.
    /// </summary>
    public class ProfessionalCompanyManager(IKeyedRepository<ProfessionalCompany> repository)
        : KeyedManager<ProfessionalCompany>(repository), IProfessionalCompanyManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<ProfessionalCompany>> GetByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(c => c.PartnerId == partnerId && c.IsActive)
                .OrderBy(c => c.Name)
                .ToListAsync(cancellationToken);
        }
    }
}
