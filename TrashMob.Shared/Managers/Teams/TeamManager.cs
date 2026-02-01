namespace TrashMob.Shared.Managers.Teams
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
    /// Manager for team operations.
    /// </summary>
    public class TeamManager : KeyedManager<Team>, ITeamManager
    {
        private readonly IKeyedRepository<TeamMember> teamMemberRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="TeamManager"/> class.
        /// </summary>
        /// <param name="repository">The team repository.</param>
        /// <param name="teamMemberRepository">The team member repository.</param>
        public TeamManager(
            IKeyedRepository<Team> repository,
            IKeyedRepository<TeamMember> teamMemberRepository)
            : base(repository)
        {
            this.teamMemberRepository = teamMemberRepository;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Team>> GetPublicTeamsAsync(
            double? latitude = null,
            double? longitude = null,
            double? radiusMiles = null,
            CancellationToken cancellationToken = default)
        {
            var teams = await Repo.Get()
                .Where(t => t.IsPublic && t.IsActive)
                .ToListAsync(cancellationToken);

            if (latitude.HasValue && longitude.HasValue && radiusMiles.HasValue)
            {
                teams = teams
                    .Where(t => t.Latitude.HasValue && t.Longitude.HasValue &&
                                CalculateDistance(latitude.Value, longitude.Value, t.Latitude.Value, t.Longitude.Value) <= radiusMiles.Value)
                    .ToList();
            }

            return teams;
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Team>> GetTeamsByUserAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var teamIds = await teamMemberRepository.Get()
                .Where(tm => tm.UserId == userId)
                .Select(tm => tm.TeamId)
                .ToListAsync(cancellationToken);

            return await Repo.Get()
                .Where(t => teamIds.Contains(t.Id) && t.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<IEnumerable<Team>> GetTeamsUserLeadsAsync(Guid userId, CancellationToken cancellationToken = default)
        {
            var teamIds = await teamMemberRepository.Get()
                .Where(tm => tm.UserId == userId && tm.IsTeamLead)
                .Select(tm => tm.TeamId)
                .ToListAsync(cancellationToken);

            return await Repo.Get()
                .Where(t => teamIds.Contains(t.Id) && t.IsActive)
                .ToListAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<bool> IsTeamNameAvailableAsync(string name, Guid? excludeTeamId = null, CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLowerInvariant();

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            var query = Repo.Get()
                .Where(t => t.Name.ToLower() == normalizedName);
#pragma warning restore CA1862

            if (excludeTeamId.HasValue)
            {
                query = query.Where(t => t.Id != excludeTeamId.Value);
            }

            return !await query.AnyAsync(cancellationToken);
        }

        /// <inheritdoc />
        public async Task<Team> GetByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            var normalizedName = name.Trim().ToLowerInvariant();

            // CA1862: ToLower() is intentional here for SQL translation in EF Core
#pragma warning disable CA1862
            return await Repo.Get()
                .FirstOrDefaultAsync(t => t.Name.ToLower() == normalizedName, cancellationToken);
#pragma warning restore CA1862
        }

        /// <summary>
        /// Calculates the distance between two geographic coordinates using the Haversine formula.
        /// </summary>
        private static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 3959; // Earth's radius in miles

            var dLat = ToRadians(lat2 - lat1);
            var dLon = ToRadians(lon2 - lon1);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(ToRadians(lat1)) * Math.Cos(ToRadians(lat2)) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c;
        }

        private static double ToRadians(double degrees)
        {
            return degrees * Math.PI / 180;
        }
    }
}
