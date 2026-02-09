namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Shared.Managers.Interfaces;
using TrashMob.Shared.Poco;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for querying TrashMob leaderboards.
/// </summary>
[McpServerToolType]
public class GetLeaderboardTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly ILeaderboardManager _leaderboardManager;

    public GetLeaderboardTool(ILeaderboardManager leaderboardManager)
    {
        _leaderboardManager = leaderboardManager;
    }

    /// <summary>
    /// Get volunteer or team leaderboard rankings.
    /// </summary>
    /// <param name="type">Leaderboard type: Events, Bags, Weight, or Hours (default: Events)</param>
    /// <param name="timeRange">Time range: Week, Month, Year, or AllTime (default: Month)</param>
    /// <param name="scope">Location scope: Global, Region, or City (default: Global)</param>
    /// <param name="location">Location value (region or city name). Required if scope is not Global.</param>
    /// <param name="entity">Entity type: User or Team (default: User)</param>
    /// <param name="maxResults">Maximum number of entries to return (default: 20, max: 50)</param>
    /// <returns>JSON leaderboard with ranked entries</returns>
    [McpServerTool]
    [Description("Get TrashMob volunteer or team leaderboard rankings. Shows top contributors by events attended, bags collected, weight collected, or hours volunteered. Filter by time range and location.")]
    public async Task<string> GetLeaderboard(
        string type = "Events",
        string timeRange = "Month",
        string scope = "Global",
        string? location = null,
        string entity = "User",
        int maxResults = 20)
    {
        var limit = Math.Min(maxResults, 50);

        var isTeam = string.Equals(entity, "Team", StringComparison.OrdinalIgnoreCase);

        LeaderboardResponse response;

        if (isTeam)
        {
            response = await _leaderboardManager.GetTeamLeaderboardAsync(
                type, timeRange, scope, location, limit);
        }
        else
        {
            response = await _leaderboardManager.GetLeaderboardAsync(
                type, timeRange, scope, location, limit);
        }

        var entries = response.Entries
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            leaderboard = new
            {
                type = response.LeaderboardType,
                time_range = response.TimeRange,
                scope = response.LocationScope,
                location = response.LocationValue,
                entity = isTeam ? "Team" : "User",
                computed_date = response.ComputedDate.ToString("O"),
                total_entries = response.TotalEntries
            },
            entries,
            search_criteria = new
            {
                type,
                time_range = timeRange,
                scope,
                location,
                entity,
                max_results = limit
            }
        }, JsonOptions);
    }

    private static LeaderboardEntryDto Sanitize(LeaderboardEntry e)
    {
        return new LeaderboardEntryDto
        {
            Rank = e.Rank,
            Name = e.EntityName,
            EntityType = e.EntityType,
            Score = e.Score,
            FormattedScore = e.FormattedScore,
            Region = e.Region,
            City = e.City
        };
    }
}
