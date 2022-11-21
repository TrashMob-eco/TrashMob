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

    public class PartnerAdminManager : BaseManager<PartnerAdmin>, IPartnerAdminManager
    {
        public PartnerAdminManager(IBaseRepository<PartnerAdmin> partnerAdminRepository) : base(partnerAdminRepository)
        {
        }

        public async Task<IEnumerable<User>> GetAdminsForPartnerAsync(Guid partnerId, CancellationToken cancellationToken)
        {
            return await Repository.Get()
                                   .Where(p => p.PartnerId == partnerId)
                                   .Include(p => p.User)
                                   .Select(p => p.User)
                                   .ToListAsync(cancellationToken);
        }

        public override async Task<IEnumerable<PartnerAdmin>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }

        public async Task<IEnumerable<Partner>> GetPartnersByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var results = await Repository.Get()
                .Where(t => t.UserId == userId)
                .Include(t => t.Partner)
                .Select(t => t.Partner)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}
