namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;

/// <summary>
/// MCP tool for getting route tracking statistics for a TrashMob event.
/// </summary>
[McpServerToolType]
public class GetEventRouteStatsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IEventAttendeeRouteManager _routeManager;
    private readonly IEventManager _eventManager;

    public GetEventRouteStatsTool(IEventAttendeeRouteManager routeManager, IEventManager eventManager)
    {
        _routeManager = routeManager;
        _eventManager = eventManager;
    }

    /// <summary>
    /// Get route tracking statistics for a specific cleanup event.
    /// </summary>
    /// <param name="eventId">The event ID (GUID format)</param>
    /// <returns>JSON with aggregated route statistics including distance, duration, coverage, and collection data</returns>
    [McpServerTool]
    [Description("Get route tracking statistics for a TrashMob cleanup event. Returns aggregated data including total distance walked, duration, area covered, number of contributors, and collection metrics (bags and weight).")]
    public async Task<string> GetEventRouteStats(
        string eventId)
    {
        if (!Guid.TryParse(eventId, out var parsedEventId))
        {
            return JsonSerializer.Serialize(new
            {
                error = "Invalid event ID format. Must be a valid GUID."
            }, JsonOptions);
        }

        var evt = await _eventManager.GetAsync(parsedEventId);
        if (evt is null || evt.EventVisibilityId != (int)EventVisibilityEnum.Public)
        {
            return JsonSerializer.Serialize(new
            {
                error = "Event not found."
            }, JsonOptions);
        }

        var stats = await _routeManager.GetEventRouteStatsAsync(parsedEventId);

        return JsonSerializer.Serialize(new
        {
            route_stats = new
            {
                total_routes = stats.TotalRoutes,
                total_distance_meters = stats.TotalDistanceMeters,
                total_duration_minutes = stats.TotalDurationMinutes,
                unique_contributors = stats.UniqueContributors,
                total_bags_collected = stats.TotalBagsCollected,
                total_weight_collected_lbs = stats.TotalWeightCollected,
                coverage_area_square_meters = stats.CoverageAreaSquareMeters
            },
            event_url = $"https://trashmob.eco/eventdetails/{parsedEventId}"
        }, JsonOptions);
    }
}
