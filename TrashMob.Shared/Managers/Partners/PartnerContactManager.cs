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
    /// Manages partner contacts including CRUD operations and retrieving the partner associated with a contact.
    /// </summary>
    public class PartnerContactManager(IKeyedRepository<PartnerContact> partnerContactRepository)
        : KeyedManager<PartnerContact>(partnerContactRepository), IPartnerContactManager
    {

        /// <inheritdoc />
        public async Task<Partner> GetPartnerForContact(Guid partnerContactId, CancellationToken cancellationToken)
        {
            var partnerContact = await Repository.Get(plc => plc.Id == partnerContactId, false)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);
            return partnerContact.Partner;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerContact>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<PartnerContact> AddAsync(PartnerContact instance, Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await base.AddAsync(instance, userId, cancellationToken);
        }
    }
}