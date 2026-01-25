namespace TrashMob.Shared.Managers.Partners
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
    /// Manages partner locations including CRUD operations and retrieving locations with their contacts.
    /// </summary>
    public class PartnerLocationManager : KeyedManager<PartnerLocation>, IPartnerLocationManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationManager"/> class.
        /// </summary>
        /// <param name="partnerLocationRepository">The repository for partner location data access.</param>
        public PartnerLocationManager(IKeyedRepository<PartnerLocation> partnerLocationRepository) : base(
            partnerLocationRepository)
        {
        }

        /// <inheritdoc />
        public async Task<Partner> GetPartnerForLocationAsync(Guid partnerLocationId,
            CancellationToken cancellationToken)
        {
            var partnerLocation = await Repository.Get(pl => pl.Id == partnerLocationId)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);

            return partnerLocation.Partner;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerLocation>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).Include(p => p.PartnerLocationContacts)
                .ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}