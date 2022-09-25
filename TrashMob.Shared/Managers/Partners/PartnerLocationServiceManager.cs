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

    public class PartnerLocationServiceManager : BaseManager<PartnerLocationService>, IBaseManager<PartnerLocationService>
    {
        public PartnerLocationServiceManager(IBaseRepository<PartnerLocationService> partnerLocationServiceRepository) : base(partnerLocationServiceRepository)
        {
        }

        public override async Task<PartnerLocationService> Get(Guid parentId, int secondId, CancellationToken cancellationToken)
        {
            return await Repository.Get().FirstOrDefaultAsync(p => p.PartnerLocationId == parentId && p.ServiceTypeId == secondId, cancellationToken: cancellationToken);
        }
    }
}
