namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manages sponsored adoption operations within a community.
    /// </summary>
    public interface ISponsoredAdoptionManager : IKeyedManager<SponsoredAdoption>
    {
        /// <summary>
        /// Gets all sponsored adoptions for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of sponsored adoptions with related entities.</returns>
        Task<IEnumerable<SponsoredAdoption>> GetByCommunityAsync(Guid partnerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all sponsored adoptions for a specific sponsor.
        /// </summary>
        /// <param name="sponsorId">The sponsor ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of sponsored adoptions.</returns>
        Task<IEnumerable<SponsoredAdoption>> GetBySponsorIdAsync(Guid sponsorId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all sponsored adoptions assigned to a specific professional company.
        /// </summary>
        /// <param name="companyId">The professional company ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of sponsored adoptions.</returns>
        Task<IEnumerable<SponsoredAdoption>> GetByCompanyIdAsync(Guid companyId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets compliance statistics for a community's sponsored adoption program.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Compliance statistics.</returns>
        Task<SponsoredAdoptionComplianceStats> GetComplianceByCommunityAsync(Guid partnerId, CancellationToken cancellationToken = default);
    }
}
