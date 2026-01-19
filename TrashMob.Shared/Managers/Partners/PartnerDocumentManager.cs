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
    /// Manages partner documents including CRUD operations and retrieving the partner associated with a document.
    /// </summary>
    public class PartnerDocumentManager : KeyedManager<PartnerDocument>, IPartnerDocumentManager
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerDocumentManager"/> class.
        /// </summary>
        /// <param name="repository">The repository for partner document data access.</param>
        public PartnerDocumentManager(IKeyedRepository<PartnerDocument> repository) : base(repository)
        {
        }

        /// <inheritdoc />
        public async Task<Partner> GetPartnerForDocument(Guid partnerDocumentId, CancellationToken cancellationToken)
        {
            var partnerDocument = await Repository.Get(plc => plc.Id == partnerDocumentId, false)
                .Include(p => p.Partner)
                .FirstOrDefaultAsync(cancellationToken);
            return partnerDocument.Partner;
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerDocument>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken))
                .AsEnumerable();
        }
    }
}