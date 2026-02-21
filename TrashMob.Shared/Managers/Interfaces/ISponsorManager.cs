namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Manages sponsor operations within a community's sponsored adoption program.
    /// </summary>
    public interface ISponsorManager : IKeyedManager<Sponsor>
    {
        /// <summary>
        /// Gets all active sponsors for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A collection of sponsors.</returns>
        Task<IEnumerable<Sponsor>> GetByCommunityAsync(Guid partnerId, CancellationToken cancellationToken = default);
    }
}
