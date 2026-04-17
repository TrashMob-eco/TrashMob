namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models.Poco;
using TrashMob.Shared.Managers.Interfaces;

/// <summary>
/// MCP tool for AI-powered community prospect discovery.
/// </summary>
[McpServerToolType]
public class DiscoverProspectsTool(IClaudeDiscoveryService discoveryService)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Discover potential community partners using AI research.
    /// </summary>
    /// <param name="prompt">Freeform research query (e.g., "Find environmental nonprofits in Portland, OR that focus on waterway cleanup"). When provided, overrides city/region/country fields.</param>
    /// <param name="city">City to search in (used when prompt is not provided)</param>
    /// <param name="region">Region/state to search in (used when prompt is not provided)</param>
    /// <param name="country">Country to search in (used when prompt is not provided, defaults to US)</param>
    /// <param name="maxResults">Maximum number of prospects to return (default: 10, max: 25)</param>
    /// <returns>JSON with discovered prospects including name, type, contact info, and rationale</returns>
    [McpServerTool]
    [Description("Discover potential community partners for TrashMob using AI research. Provide either a freeform research prompt OR city/region/country for location-based discovery. Returns organizations (municipalities, nonprofits, HOAs, civic orgs) with contact information and rationale for why they'd be good partners.")]
    public async Task<string> DiscoverProspects(
        string? prompt = null,
        string? city = null,
        string? region = null,
        string? country = null,
        int maxResults = 10)
    {
        var request = new DiscoveryRequest
        {
            Prompt = prompt,
            City = city,
            Region = region,
            Country = country,
            MaxResults = maxResults,
        };

        var result = await discoveryService.DiscoverProspectsAsync(request);

        return JsonSerializer.Serialize(new
        {
            prospects = result.Prospects,
            total_count = result.Prospects.Count,
            tokens_used = result.TokensUsed,
            message = result.Message,
        }, JsonOptions);
    }
}
