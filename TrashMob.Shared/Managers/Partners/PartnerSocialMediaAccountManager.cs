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

    public class PartnerSocialMediaAccountManager : KeyedManager<PartnerSocialMediaAccount>,
        IPartnerSocialMediaAccountManager
    {
        public PartnerSocialMediaAccountManager(IKeyedRepository<PartnerSocialMediaAccount> repository) : base(
            repository)
        {
        }

        public override async Task<IEnumerable<PartnerSocialMediaAccount>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        public async Task<Partner> GetPartnerForSocialMediaAccount(Guid partnerSocialMediaAccountId,
            CancellationToken cancellationToken)
        {
            var partnerSocialMediaAccount = await Repository.Get(ps => ps.Id == partnerSocialMediaAccountId, false)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);
            return partnerSocialMediaAccount.Partner;
        }
    }
}