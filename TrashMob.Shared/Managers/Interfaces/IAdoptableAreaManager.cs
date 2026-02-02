namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing adoptable areas within a community.
    /// </summary>
    public interface IAdoptableAreaManager : IKeyedManager<AdoptableArea>
    {
        /// <summary>
        /// Gets all adoptable areas for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of adoptable areas for the community.</returns>
        Task<IEnumerable<AdoptableArea>> GetByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all available adoptable areas for a community.
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of available adoptable areas for the community.</returns>
        Task<IEnumerable<AdoptableArea>> GetAvailableByCommunityAsync(
            Guid partnerId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if an area name is available within a community (case-insensitive).
        /// </summary>
        /// <param name="partnerId">The community (partner) ID.</param>
        /// <param name="name">The area name to check.</param>
        /// <param name="excludeAreaId">Optional area ID to exclude from the check (for updates).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the name is available; otherwise, false.</returns>
        Task<bool> IsNameAvailableAsync(
            Guid partnerId,
            string name,
            Guid? excludeAreaId = null,
            CancellationToken cancellationToken = default);
    }
}
