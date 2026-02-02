namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for retrieving TrashMob platform statistics.
/// </summary>
[McpServerToolType]
public class GetStatsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IEventSummaryManager _eventSummaryManager;

    public GetStatsTool(IEventSummaryManager eventSummaryManager)
    {
        _eventSummaryManager = eventSummaryManager;
    }

    /// <summary>
    /// Get aggregate statistics for the TrashMob platform.
    /// </summary>
    /// <returns>JSON object with platform-wide statistics including total events, participants, bags collected, hours volunteered, and weight of litter collected.</returns>
    [McpServerTool]
    [Description("Get aggregate statistics for the TrashMob volunteer platform. Returns total events held, participants involved, bags of litter collected, volunteer hours contributed, and weight of trash removed.")]
    public async Task<string> GetStats()
    {
        var stats = await _eventSummaryManager.GetStatsAsync(CancellationToken.None);

        var statsDto = new StatsDto
        {
            TotalEvents = stats.TotalEvents,
            TotalParticipants = stats.TotalParticipants,
            TotalBags = stats.TotalBags,
            TotalHours = stats.TotalHours,
            TotalWeightInPounds = stats.TotalWeightInPounds,
            TotalWeightInKilograms = stats.TotalWeightInKilograms,
            TotalLitterReportsSubmitted = stats.TotalLitterReportsSubmitted,
            TotalLitterReportsClosed = stats.TotalLitterReportsClosed
        };

        return JsonSerializer.Serialize(new
        {
            stats = statsDto,
            description = "Aggregate statistics for the TrashMob volunteer cleanup platform",
            last_updated = DateTimeOffset.UtcNow.ToString("O")
        }, JsonOptions);
    }
}
