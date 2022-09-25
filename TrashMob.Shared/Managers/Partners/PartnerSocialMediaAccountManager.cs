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

    public class PartnerSocialMediaAccountManager : KeyedManager<PartnerSocialMediaAccount>, IKeyedManager<PartnerSocialMediaAccount>
    {
        public PartnerSocialMediaAccountManager(IKeyedRepository<PartnerSocialMediaAccount> repository) : base(repository)
        {
        }

        public override async Task<IEnumerable<PartnerSocialMediaAccount>> GetByParentId(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
