namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching TrashMob community pages.
/// </summary>
[McpServerToolType]
public class SearchCommunitiesTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly ICommunityManager _communityManager;

    public SearchCommunitiesTool(ICommunityManager communityManager)
    {
        _communityManager = communityManager;
    }

    /// <summary>
    /// Search for TrashMob community pages by location.
    /// </summary>
    /// <param name="latitude">Latitude for location-based search (optional)</param>
    /// <param name="longitude">Longitude for location-based search (optional)</param>
    /// <param name="radiusMiles">Search radius in miles (default: 50, max: 200)</param>
    /// <param name="slug">Community slug to look up directly (optional, e.g., "seattle-wa")</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 50)</param>
    /// <returns>JSON array of matching communities</returns>
    [McpServerTool]
    [Description("Search for TrashMob community pages. Communities are local partnerships that organize and support cleanup events in specific areas. Each community has a dedicated page with local events and resources.")]
    public async Task<string> SearchCommunities(
        double? latitude = null,
        double? longitude = null,
        double radiusMiles = 50,
        string? slug = null,
        int maxResults = 20)
    {
        IEnumerable<Partner> communities;

        // If searching by slug, look up directly
        if (!string.IsNullOrWhiteSpace(slug))
        {
            var community = await _communityManager.GetBySlugAsync(slug);
            communities = community != null ? [community] : [];
        }
        else
        {
            // Location-based search
            var radius = Math.Min(radiusMiles, 200);
            communities = await _communityManager.GetEnabledCommunitiesAsync(latitude, longitude, radius);
        }

        var sanitizedCommunities = communities
            .Take(Math.Min(maxResults, 50))
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            communities = sanitizedCommunities,
            total_count = sanitizedCommunities.Count,
            search_criteria = new
            {
                latitude,
                longitude,
                radius_miles = radiusMiles,
                slug
            }
        }, JsonOptions);
    }

    private static CommunityDto Sanitize(Partner p)
    {
        return new CommunityDto
        {
            Id = p.Id,
            Name = p.Name,
            Slug = p.Slug ?? string.Empty,
            Tagline = p.Tagline,
            City = p.City,
            Region = p.Region,
            Country = p.Country,
            Website = p.Website,
            Url = !string.IsNullOrEmpty(p.Slug)
                ? $"https://trashmob.eco/communities/{p.Slug}"
                : $"https://trashmob.eco/communities"
        };
    }
}
