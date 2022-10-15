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

        public override async Task<PartnerService> GetAsync(Guid parentId, int secondId, CancellationToken cancellationToken)
        {
            return await Repository.Get().FirstOrDefaultAsync(p => p.PartnerId == parentId && p.ServiceTypeId == secondId, cancellationToken: cancellationToken);
        }

        public override async Task<IEnumerable<PartnerService>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }

        public override async Task<int> Delete(Guid parentId, int secondId, CancellationToken cancellationToken = default)
        {
            var instance = await Repository.Get().FirstOrDefaultAsync(p => p.PartnerId == parentId && p.ServiceTypeId == secondId, cancellationToken: cancellationToken);
            return await Repository.DeleteAsync(instance, cancellationToken);
        }
    }
}
