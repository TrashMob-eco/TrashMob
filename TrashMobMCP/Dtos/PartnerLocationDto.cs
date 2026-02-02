namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized partner location data for MCP responses.
/// Partner locations provide services to cleanup events.
/// </summary>
public class PartnerLocationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string PartnerName { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? PublicNotes { get; set; }
    public List<string> Services { get; set; } = [];
    public bool IsActive { get; set; }
}
