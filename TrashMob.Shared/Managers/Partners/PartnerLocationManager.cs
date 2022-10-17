namespace TrashMob.Shared.Managers.Partners
{
    using Microsoft.EntityFrameworkCore;
    using System;
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

        public async Task<Partner> GetPartnerForLocation(Guid partnerLocationId, CancellationToken cancellationToken)
        {
            var partnerLocation = await Repo.GetAsync(partnerLocationId, cancellationToken);
            return partnerLocation.Partner;
        }
    }
}
