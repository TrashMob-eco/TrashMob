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

    public class PartnerAdminManager : BaseManager<PartnerAdmin>, IBaseManager<PartnerAdmin>
    {
        public PartnerAdminManager(IBaseRepository<PartnerAdmin> partnerAdminRepository) : base(partnerAdminRepository)
        {
        }

        public override async Task<IEnumerable<PartnerAdmin>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
