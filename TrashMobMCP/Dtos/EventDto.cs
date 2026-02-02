namespace TrashMobMCP.Dtos;

/// <summary>
/// Sanitized event data for MCP responses.
/// Contains only public information - no PII.
/// </summary>
public class EventDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTimeOffset EventDate { get; set; }
    public int DurationHours { get; set; }
    public string City { get; set; } = string.Empty;
    public string Region { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string? EventType { get; set; }
    public int AttendeeCount { get; set; }
    public bool IsCompleted { get; set; }
    public string Url { get; set; } = string.Empty;
}
