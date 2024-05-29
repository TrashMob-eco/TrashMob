﻿namespace TrashMob.Shared.Managers.Partners
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

    public class PartnerLocationServiceManager : BaseManager<PartnerLocationService>,
        IBaseManager<PartnerLocationService>
    {
        public PartnerLocationServiceManager(IBaseRepository<PartnerLocationService> partnerLocationServiceRepository) :
            base(partnerLocationServiceRepository)
        {
        }

        public override async Task<PartnerLocationService> GetAsync(Guid parentId, int secondId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get()
                .FirstOrDefaultAsync(p => p.PartnerLocationId == parentId && p.ServiceTypeId == secondId,
                    cancellationToken);
        }

        public override async Task<IEnumerable<PartnerLocationService>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerLocationId == parentId).ToListAsync(cancellationToken))
                .AsEnumerable();
        }

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