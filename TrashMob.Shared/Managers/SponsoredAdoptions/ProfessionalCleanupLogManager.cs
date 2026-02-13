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
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for professional cleanup log operations.
    /// </summary>
    public class ProfessionalCleanupLogManager : KeyedManager<ProfessionalCleanupLog>, IProfessionalCleanupLogManager
    {
        private readonly IKeyedRepository<SponsoredAdoption> sponsoredAdoptionRepository;
        private readonly IProfessionalCompanyUserManager companyUserManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProfessionalCleanupLogManager"/> class.
        /// </summary>
        /// <param name="repository">The cleanup log repository.</param>
        /// <param name="sponsoredAdoptionRepository">The sponsored adoption repository.</param>
        /// <param name="companyUserManager">The company user manager for authorization checks.</param>
        public ProfessionalCleanupLogManager(
            IKeyedRepository<ProfessionalCleanupLog> repository,
            IKeyedRepository<SponsoredAdoption> sponsoredAdoptionRepository,
            IProfessionalCompanyUserManager companyUserManager) : base(repository)
        {
            this.sponsoredAdoptionRepository = sponsoredAdoptionRepository;
            this.companyUserManager = companyUserManager;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProfessionalCleanupLog>> GetByCompanyIdAsync(
            Guid companyId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(l => l.SponsoredAdoption)
                    .ThenInclude(sa => sa.AdoptableArea)
                .Where(l => l.ProfessionalCompanyId == companyId)
                .OrderByDescending(l => l.CleanupDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProfessionalCleanupLog>> GetBySponsorIdAsync(
            Guid sponsorId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Include(l => l.SponsoredAdoption)
                    .ThenInclude(sa => sa.AdoptableArea)
                .Where(l => l.SponsoredAdoption.SponsorId == sponsorId)
                .OrderByDescending(l => l.CleanupDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ProfessionalCleanupLog>> GetBySponsoredAdoptionIdAsync(
            Guid sponsoredAdoptionId,
            CancellationToken cancellationToken = default)
        {
            return await Repo.Get()
                .Where(l => l.SponsoredAdoptionId == sponsoredAdoptionId)
                .OrderByDescending(l => l.CleanupDate)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<ServiceResult<ProfessionalCleanupLog>> LogCleanupAsync(
            ProfessionalCleanupLog log,
            Guid userId,
            CancellationToken cancellationToken = default)
        {
            // Validate the sponsored adoption exists and is active
            var adoption = await sponsoredAdoptionRepository.GetAsync(log.SponsoredAdoptionId, cancellationToken);
            if (adoption == null)
            {
                return ServiceResult<ProfessionalCleanupLog>.Failure("Sponsored adoption not found.");
            }

            if (adoption.Status != SponsoredAdoptionStatus.Active)
            {
                return ServiceResult<ProfessionalCleanupLog>.Failure("Sponsored adoption is not active.");
            }

            // Validate the company matches the adoption's assigned company
            if (adoption.ProfessionalCompanyId != log.ProfessionalCompanyId)
            {
                return ServiceResult<ProfessionalCleanupLog>.Failure("Company is not assigned to this sponsored adoption.");
            }

            // Validate the user is a member of the professional company
            var isCompanyUser = await companyUserManager.IsCompanyUserAsync(
                log.ProfessionalCompanyId, userId, cancellationToken);

            if (!isCompanyUser)
            {
                return ServiceResult<ProfessionalCleanupLog>.Failure("User is not a member of this professional company.");
            }

            var result = await AddAsync(log, userId, cancellationToken);
            return ServiceResult<ProfessionalCleanupLog>.Success(result);
        }
    }
}
