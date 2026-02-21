namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Models.Poco;

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

        /// <summary>
        /// Gets events in a community filtered by city/region matching.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="upcomingOnly">If true, only returns upcoming events.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of events in the community.</returns>
        Task<IEnumerable<Event>> GetCommunityEventsAsync(string slug, bool upcomingOnly = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets teams near a community by location proximity.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="radiusMiles">The radius in miles to search within.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of teams near the community.</returns>
        Task<IEnumerable<Team>> GetCommunityTeamsAsync(string slug, double radiusMiles = 50, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets litter reports in a community by city/region matching.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of litter reports in the community.</returns>
        Task<IEnumerable<LitterReport>> GetCommunityLitterReportsAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets aggregated stats for a community.
        /// </summary>
        /// <param name="slug">The community slug.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Stats for the community.</returns>
        Task<Stats> GetCommunityStatsAsync(string slug, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a community by its ID (for admin operations).
        /// </summary>
        /// <param name="partnerId">The partner/community ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The community or null if not found.</returns>
        Task<Partner> GetByIdAsync(Guid partnerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates community content (for admin operations).
        /// Only updates fields that are editable by community admins.
        /// </summary>
        /// <param name="community">The community data to update.</param>
        /// <param name="userId">The ID of the user making the update.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The updated community.</returns>
        Task<Partner> UpdateCommunityContentAsync(Partner community, Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets communities that are marked as featured for display on the landing page and home page.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of featured communities.</returns>
        Task<IEnumerable<Partner>> GetFeaturedCommunitiesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets aggregate public stats across all active communities.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Aggregate community stats.</returns>
        Task<CommunityPublicStats> GetPublicStatsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets admin dashboard data for a community.
        /// </summary>
        /// <param name="partnerId">The partner/community ID.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>Dashboard data including recent activity and metrics.</returns>
        Task<CommunityDashboard> GetCommunityDashboardAsync(Guid partnerId, CancellationToken cancellationToken = default);
    }
}
