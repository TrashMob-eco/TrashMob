namespace TrashMob.Shared.Managers.Partners
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Threading;
    using System;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence.Interfaces;
    using Microsoft.EntityFrameworkCore;

    public class PartnerServiceManager : BaseManager<PartnerService>, IBaseManager<PartnerService>
    {
        public PartnerServiceManager(IBaseRepository<PartnerService> partnerServiceRepository) : base(partnerServiceRepository)
        {
        }

        public override async Task<IEnumerable<PartnerService>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }

    }
}
