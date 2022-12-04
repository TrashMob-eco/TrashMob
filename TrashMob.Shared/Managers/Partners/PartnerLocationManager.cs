namespace TrashMob.Shared.Managers.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;

    public class PartnerLocationManager : KeyedManager<PartnerLocation>, IPartnerLocationManager
    {
        public PartnerLocationManager(IKeyedRepository<PartnerLocation> partnerLocationRepository) : base(partnerLocationRepository)
        {
        }

        public async Task<Partner> GetPartnerForLocationAsync(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocation = await Repository.Get(pl => pl.Id == partnerLocationId)
                                                  .Include(p => p.Partner)
                                                  .FirstOrDefaultAsync(cancellationToken);

            return partnerLocation.Partner;
        }

        public override async Task<IEnumerable<PartnerLocation>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).Include(p => p.PartnerLocationContacts).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
