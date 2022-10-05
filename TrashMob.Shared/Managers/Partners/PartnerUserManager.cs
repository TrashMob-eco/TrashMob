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

    public class PartnerUserManager : BaseManager<PartnerUser>, IBaseManager<PartnerUser>
    {
        public PartnerUserManager(IBaseRepository<PartnerUser> partnerUserRepository) : base(partnerUserRepository)
        {
        }

        public override async Task<IEnumerable<PartnerUser>> GetByParentId(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
