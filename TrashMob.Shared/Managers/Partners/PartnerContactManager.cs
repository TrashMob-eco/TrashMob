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

    public class PartnerContactManager : KeyedManager<PartnerContact>, IPartnerContactManager
    {
        public PartnerContactManager(IKeyedRepository<PartnerContact> partnerContactRepository) : base(partnerContactRepository)
        {
        }

        public async Task<Partner> GetPartnerForContact(Guid partnerContactId, CancellationToken cancellationToken)
        {
            var partnerContact = await Repository.Get(plc => plc.Id == partnerContactId, false).FirstOrDefaultAsync(cancellationToken);
            return partnerContact.Partner;
        }

        public override async Task<IEnumerable<PartnerContact>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
