namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing community pages.
    /// Communities are partners with enabled home pages.
    /// </summary>
    public interface ICommunityManager
    {
        /// <summary>
        /// Gets all communities with enabled home pages, optionally filtered by location.
        /// </summary>
        /// <param name="latitude">Optional latitude for location-based filtering.</param>
        /// <param name="longitude">Optional longitude for location-based filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles for location search.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of communities with enabled home pages.</returns>
        Task<IEnumerable<Partner>> GetEnabledCommunitiesAsync(
            double? latitude = null,
            double? longitude = null,
            double? radiusMiles = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a community by its URL slug.
        /// </summary>
        /// <param name="slug">The URL-friendly slug (e.g., "seattle-wa").</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The community or null if not found or not enabled.</returns>
        Task<Partner> GetBySlugAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a slug is available (case-insensitive).
        /// </summary>
        /// <param name="slug">The slug to check.</param>
        /// <param name="excludePartnerId">Optional partner ID to exclude from the check (for updates).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the slug is available; otherwise, false.</returns>
        Task<bool> IsSlugAvailableAsync(string slug, Guid? excludePartnerId = null, CancellationToken cancellationToken = default);
    }
}
