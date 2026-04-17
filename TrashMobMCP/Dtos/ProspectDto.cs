namespace TrashMobMCP.Dtos;

/// <summary>
/// MCP-safe representation of a community prospect in the pipeline.
/// </summary>
public class ProspectDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Type { get; set; } = string.Empty;

    public string City { get; set; } = string.Empty;

    public string Region { get; set; } = string.Empty;

    public string Country { get; set; } = string.Empty;

    public int? Population { get; set; }

    public string? Website { get; set; }

    public string? ContactName { get; set; }

    public string? ContactEmail { get; set; }

    public string? ContactTitle { get; set; }

    public int PipelineStage { get; set; }

    public string PipelineStageName { get; set; } = string.Empty;

    public int FitScore { get; set; }

    public DateTimeOffset? LastContactedDate { get; set; }

    public DateTimeOffset? NextFollowUpDate { get; set; }

    public string? Notes { get; set; }
}
