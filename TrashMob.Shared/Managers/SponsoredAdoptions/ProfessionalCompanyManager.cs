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
    public class ProfessionalCompanyManager : KeyedManager<ProfessionalCompany>, IProfessionalCompanyManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProfessionalCompanyManager"/> class.
        /// </summary>
        /// <param name="repository">The professional company repository.</param>
        public ProfessionalCompanyManager(IKeyedRepository<ProfessionalCompany> repository) : base(repository)
        {
        }

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
