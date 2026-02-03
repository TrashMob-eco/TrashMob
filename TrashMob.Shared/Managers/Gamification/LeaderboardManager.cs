namespace TrashMob.Shared.Managers.Gamification
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;
    using TrashMob.Shared.Persistence;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Manager for retrieving leaderboard data from the pre-computed cache.
    /// </summary>
    public class LeaderboardManager : ILeaderboardManager
    {
        private static readonly string[] TimeRanges = { "Week", "Month", "Year", "AllTime" };
        private static readonly string[] LeaderboardTypes = { "Events", "Bags", "Weight", "Hours" };
        private static readonly string[] LocationScopes = { "Global", "Region", "City" };

        private readonly MobDbContext dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeaderboardManager"/> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public LeaderboardManager(MobDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        /// <inheritdoc />
        public async Task<LeaderboardResponse> GetLeaderboardAsync(
            string leaderboardType,
            string timeRange = "Month",
            string locationScope = "Global",
            string locationValue = null,
            int limit = 50,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (!LeaderboardTypes.Contains(leaderboardType, StringComparer.OrdinalIgnoreCase))
            {
                leaderboardType = "Events";
            }

            if (!TimeRanges.Contains(timeRange, StringComparer.OrdinalIgnoreCase))
            {
                timeRange = "Month";
            }

            if (!LocationScopes.Contains(locationScope, StringComparer.OrdinalIgnoreCase))
            {
                locationScope = "Global";
            }

            if (limit <= 0 || limit > 100)
            {
                limit = 50;
            }

            // Query the cache
            var query = dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "User"
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == locationScope);

            if (locationScope != "Global" && !string.IsNullOrEmpty(locationValue))
            {
                query = query.Where(l => l.LocationValue == locationValue);
            }
            else if (locationScope == "Global")
            {
                query = query.Where(l => l.LocationValue == null || l.LocationValue == "");
            }

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var entries = await query
                .OrderBy(l => l.Rank)
                .Take(limit)
                .Select(l => new LeaderboardEntry
                {
                    EntityId = l.EntityId,
                    EntityName = l.EntityName,
                    EntityType = l.EntityType,
                    Rank = l.Rank,
                    Score = l.Score,
                    FormattedScore = FormatScore(l.Score, leaderboardType)
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var computedDate = await query
                .Select(l => l.ComputedDate)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return new LeaderboardResponse
            {
                LeaderboardType = leaderboardType,
                TimeRange = timeRange,
                LocationScope = locationScope,
                LocationValue = locationValue,
                ComputedDate = computedDate,
                TotalEntries = totalCount,
                Entries = entries
            };
        }

        /// <inheritdoc />
        public async Task<UserRankResponse> GetUserRankAsync(
            Guid userId,
            string leaderboardType = "Events",
            string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (!LeaderboardTypes.Contains(leaderboardType, StringComparer.OrdinalIgnoreCase))
            {
                leaderboardType = "Events";
            }

            if (!TimeRanges.Contains(timeRange, StringComparer.OrdinalIgnoreCase))
            {
                timeRange = "AllTime";
            }

            // Check if user exists and is eligible
            var user = await dbContext.Users
                .AsNoTracking()
                .Where(u => u.Id == userId)
                .Select(u => new { u.ShowOnLeaderboards })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (user == null)
            {
                return new UserRankResponse
                {
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    IsEligible = false,
                    IneligibleReason = "User not found."
                };
            }

            if (!user.ShowOnLeaderboards)
            {
                return new UserRankResponse
                {
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    IsEligible = false,
                    IneligibleReason = "You have opted out of appearing on leaderboards."
                };
            }

            // Get total ranked count
            var totalRanked = await dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "User"
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == "Global")
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);

            // Get user's entry
            var userEntry = await dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "User"
                    && l.EntityId == userId
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == "Global")
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (userEntry == null)
            {
                // User not ranked - check why
                var eventCount = await GetUserEventCountAsync(userId, timeRange, cancellationToken).ConfigureAwait(false);
                var ineligibleReason = eventCount < 3
                    ? $"You need to attend at least 3 events to appear on leaderboards. You have attended {eventCount} event(s)."
                    : "You are not yet ranked on this leaderboard.";

                return new UserRankResponse
                {
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    TotalRanked = totalRanked,
                    IsEligible = eventCount >= 3,
                    IneligibleReason = eventCount < 3 ? ineligibleReason : null
                };
            }

            return new UserRankResponse
            {
                LeaderboardType = leaderboardType,
                TimeRange = timeRange,
                Rank = userEntry.Rank,
                Score = userEntry.Score,
                FormattedScore = FormatScore(userEntry.Score, leaderboardType),
                TotalRanked = totalRanked,
                IsEligible = true
            };
        }

        /// <inheritdoc />
        public string[] GetAvailableTimeRanges() => TimeRanges;

        /// <inheritdoc />
        public string[] GetAvailableLeaderboardTypes() => LeaderboardTypes;

        /// <inheritdoc />
        public string[] GetAvailableLocationScopes() => LocationScopes;

        private static string FormatScore(decimal score, string leaderboardType)
        {
            return leaderboardType.ToLowerInvariant() switch
            {
                "events" => $"{(int)score} event{((int)score != 1 ? "s" : "")}",
                "bags" => $"{(int)score} bag{((int)score != 1 ? "s" : "")}",
                "weight" => $"{score:N1} lbs",
                "hours" => $"{score:N1} hours",
                _ => score.ToString("N0")
            };
        }

        private async Task<int> GetUserEventCountAsync(Guid userId, string timeRange, CancellationToken cancellationToken)
        {
            var query = dbContext.EventAttendees
                .AsNoTracking()
                .Where(ea => ea.UserId == userId);

            if (timeRange != "AllTime")
            {
                var startDate = GetStartDateForTimeRange(timeRange);
                query = query.Where(ea => ea.Event.EventDate >= startDate);
            }

            return await query
                .Select(ea => ea.EventId)
                .Distinct()
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }

        private static DateTimeOffset GetStartDateForTimeRange(string timeRange)
        {
            var now = DateTimeOffset.UtcNow;
            return timeRange.ToLowerInvariant() switch
            {
                "week" => now.AddDays(-7),
                "month" => now.AddMonths(-1),
                "year" => now.AddYears(-1),
                _ => DateTimeOffset.MinValue
            };
        }

        /// <inheritdoc />
        public async Task<LeaderboardResponse> GetTeamLeaderboardAsync(
            string leaderboardType,
            string timeRange = "Month",
            string locationScope = "Global",
            string locationValue = null,
            int limit = 50,
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (!LeaderboardTypes.Contains(leaderboardType, StringComparer.OrdinalIgnoreCase))
            {
                leaderboardType = "Events";
            }

            if (!TimeRanges.Contains(timeRange, StringComparer.OrdinalIgnoreCase))
            {
                timeRange = "Month";
            }

            if (!LocationScopes.Contains(locationScope, StringComparer.OrdinalIgnoreCase))
            {
                locationScope = "Global";
            }

            if (limit <= 0 || limit > 100)
            {
                limit = 50;
            }

            // Query the cache for team leaderboards
            var query = dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "Team"
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == locationScope);

            if (locationScope != "Global" && !string.IsNullOrEmpty(locationValue))
            {
                query = query.Where(l => l.LocationValue == locationValue);
            }
            else if (locationScope == "Global")
            {
                query = query.Where(l => l.LocationValue == null || l.LocationValue == "");
            }

            var totalCount = await query.CountAsync(cancellationToken).ConfigureAwait(false);

            var entries = await query
                .OrderBy(l => l.Rank)
                .Take(limit)
                .Select(l => new LeaderboardEntry
                {
                    EntityId = l.EntityId,
                    EntityName = l.EntityName,
                    EntityType = l.EntityType,
                    Rank = l.Rank,
                    Score = l.Score,
                    FormattedScore = FormatScore(l.Score, leaderboardType)
                })
                .ToListAsync(cancellationToken)
                .ConfigureAwait(false);

            var computedDate = await query
                .Select(l => l.ComputedDate)
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            return new LeaderboardResponse
            {
                LeaderboardType = leaderboardType,
                TimeRange = timeRange,
                LocationScope = locationScope,
                LocationValue = locationValue,
                ComputedDate = computedDate,
                TotalEntries = totalCount,
                Entries = entries
            };
        }

        /// <inheritdoc />
        public async Task<TeamRankResponse> GetTeamRankAsync(
            Guid teamId,
            string leaderboardType = "Events",
            string timeRange = "AllTime",
            CancellationToken cancellationToken = default)
        {
            // Validate parameters
            if (!LeaderboardTypes.Contains(leaderboardType, StringComparer.OrdinalIgnoreCase))
            {
                leaderboardType = "Events";
            }

            if (!TimeRanges.Contains(timeRange, StringComparer.OrdinalIgnoreCase))
            {
                timeRange = "AllTime";
            }

            // Check if team exists and is active
            var team = await dbContext.Teams
                .AsNoTracking()
                .Where(t => t.Id == teamId)
                .Select(t => new { t.Name, t.IsActive })
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (team == null)
            {
                return new TeamRankResponse
                {
                    TeamId = teamId,
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    IsEligible = false,
                    IneligibleReason = "Team not found."
                };
            }

            if (!team.IsActive)
            {
                return new TeamRankResponse
                {
                    TeamId = teamId,
                    TeamName = team.Name,
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    IsEligible = false,
                    IneligibleReason = "This team is no longer active."
                };
            }

            // Get total ranked count
            var totalRanked = await dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "Team"
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == "Global")
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);

            // Get team's entry
            var teamEntry = await dbContext.LeaderboardCaches
                .AsNoTracking()
                .Where(l => l.EntityType == "Team"
                    && l.EntityId == teamId
                    && l.LeaderboardType == leaderboardType
                    && l.TimeRange == timeRange
                    && l.LocationScope == "Global")
                .FirstOrDefaultAsync(cancellationToken)
                .ConfigureAwait(false);

            if (teamEntry == null)
            {
                // Team not ranked - check why
                var eventCount = await GetTeamEventCountAsync(teamId, timeRange, cancellationToken).ConfigureAwait(false);
                var ineligibleReason = eventCount < 3
                    ? $"Teams need at least 3 events with member participation to appear on leaderboards. This team has {eventCount} event(s)."
                    : "This team is not yet ranked on this leaderboard.";

                return new TeamRankResponse
                {
                    TeamId = teamId,
                    TeamName = team.Name,
                    LeaderboardType = leaderboardType,
                    TimeRange = timeRange,
                    TotalRanked = totalRanked,
                    IsEligible = eventCount >= 3,
                    IneligibleReason = eventCount < 3 ? ineligibleReason : null
                };
            }

            return new TeamRankResponse
            {
                TeamId = teamId,
                TeamName = team.Name,
                LeaderboardType = leaderboardType,
                TimeRange = timeRange,
                Rank = teamEntry.Rank,
                Score = teamEntry.Score,
                FormattedScore = FormatScore(teamEntry.Score, leaderboardType),
                TotalRanked = totalRanked,
                IsEligible = true
            };
        }

        private async Task<int> GetTeamEventCountAsync(Guid teamId, string timeRange, CancellationToken cancellationToken)
        {
            // Count distinct events where any team member participated
            var query = dbContext.TeamMembers
                .AsNoTracking()
                .Where(tm => tm.TeamId == teamId)
                .SelectMany(tm => tm.User.EventAttendees)
                .Select(ea => new { ea.EventId, ea.Event.EventDate });

            if (timeRange != "AllTime")
            {
                var startDate = GetStartDateForTimeRange(timeRange);
                query = query.Where(x => x.EventDate >= startDate);
            }

            return await query
                .Select(x => x.EventId)
                .Distinct()
                .CountAsync(cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
