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

    public class PartnerAdminManager : BaseManager<PartnerAdmin>, IPartnerAdminManager
    {
        private readonly IKeyedRepository<Partner> partnerRepository;

        public PartnerAdminManager(IBaseRepository<PartnerAdmin> partnerAdminRepository, IKeyedRepository<Partner> partnerRepository) : base(partnerAdminRepository)
        {
            this.partnerRepository = partnerRepository;
        }

        public async Task<IEnumerable<User>> GetAdminsForPartnerAsync(Guid partnerId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get()
                .Where(p => p.PartnerId == partnerId)
                .Include(p => p.User)
                .Select(p => p.User)
                .ToListAsync(cancellationToken);
        }

        public override async Task<IEnumerable<PartnerAdmin>> GetByParentIdAsync(Guid parentId,
            CancellationToken cancellationToken)
        {
            return (await Repository.Get().Where(p => p.PartnerId == parentId).ToListAsync(cancellationToken))
                .AsEnumerable();
        }

        public async Task<IEnumerable<Partner>> GetPartnersByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var results = await Repository.Get()
                .Where(t => t.UserId == userId || t.Partner.CreatedByUserId == userId)
                .Include(t => t.Partner)
                .Select(t => t.Partner)
                .ToListAsync(cancellationToken);

            var partners = partnerRepository.Get()
                .Where(p => p.CreatedByUserId == userId)
                .ToList();

            results.AddRange(partners);
            results = results.Distinct().ToList();

            return results;
        }

        public async Task<IEnumerable<PartnerLocation>> GetHaulingPartnerLocationsByUserIdAsync(Guid userId,
            CancellationToken cancellationToken = default)
        {
            var results = await Repository.Get()
                .Where(t => t.UserId == userId && t.Partner.PartnerLocations.Any(pl =>
                    pl.PartnerLocationServices.Any(pls => pls.ServiceTypeId == (int)ServiceTypeEnum.Hauling)))
                .Include(t => t.Partner.PartnerLocations)
                .SelectMany(t => t.Partner.PartnerLocations)
                .ToListAsync(cancellationToken);

            return results;
        }
    }
}