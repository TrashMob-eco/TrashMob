namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for listing available TrashMob achievements.
/// </summary>
[McpServerToolType]
public class GetAchievementTypesTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IAchievementManager _achievementManager;

    public GetAchievementTypesTool(IAchievementManager achievementManager)
    {
        _achievementManager = achievementManager;
    }

    /// <summary>
    /// Get all available achievement types that volunteers can earn.
    /// </summary>
    /// <param name="category">Filter by category: Participation, Impact, Special, or leave empty for all (optional)</param>
    /// <returns>JSON array of achievement types with descriptions and point values</returns>
    [McpServerTool]
    [Description("Get available TrashMob achievement badges that volunteers can earn. Each achievement has criteria like attending events, collecting bags, or reaching milestones. Filter by category to see specific types.")]
    public async Task<string> GetAchievementTypes(
        string? category = null)
    {
        var achievementTypes = await _achievementManager.GetAchievementTypesAsync();

        var sanitized = achievementTypes
            .Where(a => a.IsActive != false)
            .Where(a => string.IsNullOrWhiteSpace(category)
                || string.Equals(a.Category, category, StringComparison.OrdinalIgnoreCase))
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            achievement_types = sanitized,
            total_count = sanitized.Count,
            search_criteria = new
            {
                category
            }
        }, JsonOptions);
    }

    private static AchievementTypeDto Sanitize(AchievementType a)
    {
        return new AchievementTypeDto
        {
            Name = a.DisplayName ?? a.Name,
            Description = a.Description,
            Category = a.Category,
            IconUrl = a.IconUrl,
            Points = a.Points,
            Criteria = a.Criteria
        };
    }
}
