namespace TrashMob.Shared.Managers.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using TrashMob.Models;

    /// <summary>
    /// Defines operations for managing teams.
    /// </summary>
    public interface ITeamManager : IKeyedManager<Team>
    {
        /// <summary>
        /// Gets all public teams, optionally filtered by location.
        /// </summary>
        /// <param name="latitude">Optional latitude for location-based filtering.</param>
        /// <param name="longitude">Optional longitude for location-based filtering.</param>
        /// <param name="radiusMiles">Optional radius in miles for location search.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of public teams.</returns>
        Task<IEnumerable<Team>> GetPublicTeamsAsync(
            double? latitude = null,
            double? longitude = null,
            double? radiusMiles = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all teams that a user is a member of.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of teams the user is a member of.</returns>
        Task<IEnumerable<Team>> GetTeamsByUserAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all teams that a user leads.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of teams the user leads.</returns>
        Task<IEnumerable<Team>> GetTeamsUserLeadsAsync(Guid userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Checks if a team name is available (case-insensitive).
        /// </summary>
        /// <param name="name">The team name to check.</param>
        /// <param name="excludeTeamId">Optional team ID to exclude from the check (for updates).</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>True if the name is available; otherwise, false.</returns>
        Task<bool> IsTeamNameAvailableAsync(string name, Guid? excludeTeamId = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets a team by name (case-insensitive).
        /// </summary>
        /// <param name="name">The team name.</param>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>The team or null if not found.</returns>
        Task<Team> GetByNameAsync(string name, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets all teams including private and inactive ones. Admin only.
        /// </summary>
        /// <param name="cancellationToken">A token to cancel the asynchronous operation.</param>
        /// <returns>A collection of all teams.</returns>
        Task<IEnumerable<Team>> GetAllTeamsAsync(CancellationToken cancellationToken = default);
    }
}
