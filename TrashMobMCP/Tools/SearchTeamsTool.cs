namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching TrashMob volunteer teams.
/// </summary>
[McpServerToolType]
public class SearchTeamsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly ITeamManager _teamManager;

    public SearchTeamsTool(ITeamManager teamManager)
    {
        _teamManager = teamManager;
    }

    /// <summary>
    /// Search for volunteer teams by location or name.
    /// </summary>
    /// <param name="latitude">Latitude for location-based search (optional)</param>
    /// <param name="longitude">Longitude for location-based search (optional)</param>
    /// <param name="radiusMiles">Search radius in miles (default: 25, max: 100)</param>
    /// <param name="name">Team name to search for (optional, partial match)</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 50)</param>
    /// <returns>JSON array of matching public teams</returns>
    [McpServerTool]
    [Description("Search for TrashMob volunteer teams by location or name. Teams are groups of volunteers who organize and participate in cleanup events together.")]
    public async Task<string> SearchTeams(
        double? latitude = null,
        double? longitude = null,
        double radiusMiles = 25,
        string? name = null,
        int maxResults = 20)
    {
        IEnumerable<Team> teams;

        // If searching by name, use name lookup
        if (!string.IsNullOrWhiteSpace(name))
        {
            var team = await _teamManager.GetByNameAsync(name);
            teams = team != null ? [team] : [];
        }
        else
        {
            // Location-based search
            var radius = Math.Min(radiusMiles, 100);
            teams = await _teamManager.GetPublicTeamsAsync(latitude, longitude, radius);
        }

        var sanitizedTeams = teams
            .Where(t => t.IsActive)
            .Take(Math.Min(maxResults, 50))
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            teams = sanitizedTeams,
            total_count = sanitizedTeams.Count,
            search_criteria = new
            {
                latitude,
                longitude,
                radius_miles = radiusMiles,
                name
            }
        }, JsonOptions);
    }

    private static TeamDto Sanitize(Team t)
    {
        return new TeamDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            City = t.City,
            Region = t.Region,
            Country = t.Country,
            MemberCount = t.Members?.Count ?? 0,
            IsActive = t.IsActive,
            Url = $"https://trashmob.eco/teams/{t.Id}"
        };
    }
}
