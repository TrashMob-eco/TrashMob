namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Manages professional cleanup company operations within a community.
    /// </summary>
    public interface IProfessionalCompanyManager : IKeyedManager<ProfessionalCompany>
    {
        /// <summary>
        /// Gets all active professional companies for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of professional companies.</returns>
        Task<IEnumerable<ProfessionalCompany>> GetByCommunityAsync(Guid partnerId, CancellationToken cancellationToken = default);
    }
}
