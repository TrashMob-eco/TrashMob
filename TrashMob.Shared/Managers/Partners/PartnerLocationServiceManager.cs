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
    /// Manages services offered at partner locations including CRUD operations by location and service type.
    /// </summary>
    public class PartnerLocationServiceManager : BaseManager<PartnerLocationService>,
        IBaseManager<PartnerLocationService>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocationServiceManager"/> class.
        /// </summary>
        /// <param name="partnerLocationServiceRepository">The repository for partner location service data access.</param>
        public PartnerLocationServiceManager(IBaseRepository<PartnerLocationService> partnerLocationServiceRepository) :
            base(partnerLocationServiceRepository)
        {
        }

        /// <inheritdoc />
        public override async Task<PartnerLocationService> GetAsync(Guid parentId, int secondId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get()
                .FirstOrDefaultAsync(p => p.PartnerLocationId == parentId && p.ServiceTypeId == secondId,
                    cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<PartnerLocationService>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get().Where(p => p.PartnerLocationId == parentId).ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> Delete(Guid parentId, int secondId,
            CancellationToken cancellationToken = default)
        {
            var instance = await Repository.Get()
                .FirstOrDefaultAsync(p => p.PartnerLocationId == parentId && p.ServiceTypeId == secondId,
                    cancellationToken);
            return await Repository.DeleteAsync(instance);
        }
    }
}