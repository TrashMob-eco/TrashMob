namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching TrashMob cleanup events.
/// </summary>
[McpServerToolType]
public class SearchEventsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IEventManager _eventManager;

    public SearchEventsTool(IEventManager eventManager)
    {
        _eventManager = eventManager;
    }

    /// <summary>
    /// Search for cleanup events by location, date, and status.
    /// </summary>
    /// <param name="city">City to search in (optional)</param>
    /// <param name="region">State/region to search in (optional)</param>
    /// <param name="country">Country to search in (optional, defaults to "United States")</param>
    /// <param name="startDate">Only include events starting after this date (optional, ISO 8601 format)</param>
    /// <param name="endDate">Only include events starting before this date (optional, ISO 8601 format)</param>
    /// <param name="includeCompleted">Include completed events (default: false, only shows upcoming)</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 50)</param>
    /// <returns>JSON array of matching events with public information only</returns>
    [McpServerTool]
    [Description("Search for TrashMob cleanup events by location and date. Returns upcoming volunteer events where people gather to pick up litter and clean up their communities.")]
    public async Task<string> SearchEvents(
        string? city = null,
        string? region = null,
        string? country = null,
        string? startDate = null,
        string? endDate = null,
        bool includeCompleted = false,
        int maxResults = 20)
    {
        var filter = new EventFilter
        {
            City = city,
            Region = region,
            Country = country ?? "United States",
            StartDate = ParseDate(startDate),
            EndDate = ParseDate(endDate),
            PageSize = Math.Min(maxResults, 50),
            PageIndex = 0
        };

        // Default to upcoming events only
        if (!includeCompleted)
        {
            filter.StartDate ??= DateTimeOffset.UtcNow;
        }

        var events = await _eventManager.GetFilteredEventsAsync(filter);

        var sanitizedEvents = events
            .Where(e => e.EventStatusId != (int)EventStatusEnum.Canceled)
            .Where(e => e.EventVisibilityId == (int)EventVisibilityEnum.Public)
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            events = sanitizedEvents,
            total_count = sanitizedEvents.Count,
            search_criteria = new
            {
                city,
                region,
                country = filter.Country,
                start_date = filter.StartDate?.ToString("O"),
                end_date = filter.EndDate?.ToString("O"),
                include_completed = includeCompleted
            }
        }, JsonOptions);
    }

    private static DateTimeOffset? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTimeOffset.TryParse(dateString, out var date))
            return date;

        return null;
    }

    private static EventDto Sanitize(Event e)
    {
        return new EventDto
        {
            Id = e.Id,
            Name = e.Name,
            Description = e.Description,
            EventDate = e.EventDate,
            DurationHours = e.DurationHours,
            City = e.City,
            Region = e.Region,
            Country = e.Country,
            EventType = e.EventType?.Name,
            AttendeeCount = e.EventAttendees?.Count ?? 0,
            IsCompleted = e.EventStatusId == (int)EventStatusEnum.Complete,
            Url = $"https://trashmob.eco/eventdetails/{e.Id}"
        };
    }
}
