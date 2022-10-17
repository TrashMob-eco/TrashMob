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

    public class PartnerLocationContactManager : KeyedManager<PartnerLocationContact>, IPartnerLocationContactManager
    {
        public PartnerLocationContactManager(IKeyedRepository<PartnerLocationContact> partnerLocationContactRepository) : base(partnerLocationContactRepository)
        {
        }

        public async Task<Partner> GetPartnerForLocationContact(Guid partnerLocationContactId, CancellationToken cancellationToken)
        {
            var partnerLocationContact = await Repository.Get(plc => plc.Id == partnerLocationContactId, false).FirstOrDefaultAsync(cancellationToken);
            return partnerLocationContact.PartnerLocation.Partner;
        }

        public override async Task<IEnumerable<PartnerLocationContact>> GetByParentIdAsync(Guid parentId, CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerLocationId == parentId).ToListAsync(cancellationToken)).AsEnumerable();
        }
    }
}
