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

    /// <summary>
    /// Manages partner social media accounts including CRUD operations and retrieving the associated partner.
    /// </summary>
    public class PartnerSocialMediaAccountManager : KeyedManager<PartnerSocialMediaAccount>,
        IPartnerSocialMediaAccountManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerSocialMediaAccountManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for partner social media account data access.</param>
        public PartnerSocialMediaAccountManager(IKeyedRepository<PartnerSocialMediaAccount> repository) : base(
            repository)
        {
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerSocialMediaAccount>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
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