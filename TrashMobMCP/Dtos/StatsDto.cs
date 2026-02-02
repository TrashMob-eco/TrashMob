namespace TrashMobMCP.Dtos;

/// <summary>
/// Aggregate statistics for MCP responses.
/// </summary>
public class StatsDto
{
    public int TotalEvents { get; set; }
    public int TotalParticipants { get; set; }
    public int TotalBags { get; set; }
    public int TotalHours { get; set; }
    public decimal TotalWeightInPounds { get; set; }
    public decimal TotalWeightInKilograms { get; set; }
    public int TotalLitterReportsSubmitted { get; set; }
    public int TotalLitterReportsClosed { get; set; }
}
