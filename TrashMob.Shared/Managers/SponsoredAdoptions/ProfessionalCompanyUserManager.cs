namespace TrashMob.Shared.Managers.SponsoredAdoptions
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
    /// Manager for user assignments to professional cleanup companies.
    /// Uses composite key pattern (like PartnerAdminManager).
    /// </summary>
    public class ProfessionalCompanyUserManager(IBaseRepository<ProfessionalCompanyUser> repository)
        : BaseManager<ProfessionalCompanyUser>(repository), IProfessionalCompanyUserManager
    {

        /// <inheritdoc />
        public async Task<IEnumerable<User>> GetUsersForCompanyAsync(
            Guid companyId,
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(cu => cu.ProfessionalCompanyId == companyId)
                .Include(cu => cu.User)
                .Select(cu => cu.User)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProfessionalCompany>> GetCompaniesByUserIdAsync(
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .Where(cu => cu.UserId == userId)
                .Include(cu => cu.ProfessionalCompany)
                .Select(cu => cu.ProfessionalCompany)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsCompanyUserAsync(
            Guid companyId,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            return await Repository.Get()
                .AnyAsync(cu => cu.ProfessionalCompanyId == companyId && cu.UserId == userId, cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<IEnumerable<ProfessionalCompanyUser>> GetByParentIdAsync(
            Guid parentId,
            CancellationToken cancellationToken)
        {
            return await Repository.Get()
                .Where(cu => cu.ProfessionalCompanyId == parentId)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public override async Task<int> Delete(Guid companyId, Guid userId, CancellationToken cancellationToken)
        {
            var entity = await Repository.Get(cu => cu.ProfessionalCompanyId == companyId && cu.UserId == userId, false)
                .FirstOrDefaultAsync(cancellationToken);

            if (entity == null)
            {
                return 0;
            }

            return await Repository.DeleteAsync(entity);
        }
    }
}
