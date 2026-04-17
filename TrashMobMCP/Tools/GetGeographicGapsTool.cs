namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Shared.Managers.Interfaces;

/// <summary>
/// MCP tool for finding geographic areas with events but no community partner.
/// </summary>
[McpServerToolType]
public class GetGeographicGapsTool(IProspectScoringManager scoringManager)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    /// <summary>
    /// Find geographic areas that have TrashMob events but no active community partner.
    /// </summary>
    /// <returns>JSON array of geographic gaps with event counts and nearest partner distances</returns>
    [McpServerTool]
    [Description("Find geographic areas where TrashMob events have been held but there is no active community partner. Useful for identifying high-potential areas for new community partnerships. Returns event counts, nearest partner distance, and any existing prospect in the pipeline.")]
    public async Task<string> GetGeographicGaps()
    {
        var gaps = (await scoringManager.GetGeographicGapsAsync()).ToList();

        return JsonSerializer.Serialize(new
        {
            gaps,
            total_count = gaps.Count,
        }, JsonOptions);
    }
}
