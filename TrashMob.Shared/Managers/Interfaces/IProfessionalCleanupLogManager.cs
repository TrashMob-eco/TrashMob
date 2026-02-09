namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages professional cleanup log operations.
    /// </summary>
    public interface IProfessionalCleanupLogManager : IKeyedManager<ProfessionalCleanupLog>
    {
        /// <summary>
        /// Gets all cleanup logs for a professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of cleanup logs.</returns>
        Task<IEnumerable<ProfessionalCleanupLog>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all cleanup logs across all adoptions for a specific sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of cleanup logs with navigation properties.</returns>
        Task<IEnumerable<ProfessionalCleanupLog>> GetBySponsorIdAsync(Guid sponsorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all cleanup logs for a specific sponsored adoption.
        /// </summary>
        /// <param name="sponsoredAdoptionId">The sponsored adoption ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of cleanup logs.</returns>
        Task<IEnumerable<ProfessionalCleanupLog>> GetBySponsoredAdoptionIdAsync(Guid sponsoredAdoptionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Logs a professional cleanup with validation.
        /// </summary>
        /// <param name="log">The cleanup log to create.</param>
        /// <param name="userId">The user creating the log.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A service result with the created log or error message.</returns>
        Task<ServiceResult<ProfessionalCleanupLog>> LogCleanupAsync(ProfessionalCleanupLog log, Guid userId, CancellationToken cancellationToken = default);
    }
}
