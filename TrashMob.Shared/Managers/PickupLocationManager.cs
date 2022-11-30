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

    public class PickupLocationManager : KeyedManager<PickupLocation>, IKeyedManager<PickupLocation>
    {
        public PickupLocationManager(IKeyedRepository<PickupLocation> pickupLocationRepository) : base(pickupLocationRepository)
        {
        }

        public override async Task<IEnumerable<PickupLocation>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.EventId == parentId)
                                          .ToListAsync(cancellationToken))
                                          .AsEnumerable();
        }
    }
}
