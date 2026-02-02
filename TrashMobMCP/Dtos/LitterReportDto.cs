namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized litter report data for MCP responses.
/// Contains only public location and status information.
/// </summary>
public class LitterReportDto
{
    public Guid Id { get; set; }
    public string? Description { get; set; }
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTimeOffset CreatedDate { get; set; }
    public string Url { get; set; } = string.Empty;
}
