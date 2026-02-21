namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Models.Poco;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching litter reports.
/// </summary>
[McpServerToolType]
public class SearchLitterReportsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly ILitterReportManager _litterReportManager;

    public SearchLitterReportsTool(ILitterReportManager litterReportManager)
    {
        _litterReportManager = litterReportManager;
    }

    /// <summary>
    /// Search for litter reports by location and status.
    /// </summary>
    /// <param name="city">City to search in (optional)</param>
    /// <param name="region">State/region to search in (optional)</param>
    /// <param name="country">Country to search in (optional, defaults to "United States")</param>
    /// <param name="status">Status filter: "new", "assigned", "cleaned", or "all" (default: "new")</param>
    /// <param name="startDate">Only include reports created after this date (optional, ISO 8601 format)</param>
    /// <param name="endDate">Only include reports created before this date (optional, ISO 8601 format)</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 50)</param>
    /// <returns>JSON array of matching litter reports with location and status information</returns>
    [McpServerTool]
    [Description("Search for litter reports submitted by TrashMob users. Litter reports are locations where someone has spotted litter that needs to be cleaned up. They can be new (unaddressed), assigned to an event, or already cleaned.")]
    public async Task<string> SearchLitterReports(
        string? city = null,
        string? region = null,
        string? country = null,
        string status = "new",
        string? startDate = null,
        string? endDate = null,
        int maxResults = 20)
    {
        var filter = new LitterReportFilter
        {
            City = city,
            Region = region,
            Country = country ?? "United States",
            StartDate = ParseDate(startDate),
            EndDate = ParseDate(endDate),
            LitterReportStatusId = ParseStatus(status),
            PageSize = Math.Min(maxResults, 50),
            PageIndex = 0,
            IncludeLitterImages = true // Need images for location data
        };

        var reports = await _litterReportManager.GetFilteredLitterReportsAsync(filter);

        var sanitizedReports = reports
            .Where(r => r.LitterReportStatusId != (int)LitterReportStatusEnum.Cancelled)
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            litter_reports = sanitizedReports,
            total_count = sanitizedReports.Count,
            search_criteria = new
            {
                city,
                region,
                country = filter.Country,
                status,
                start_date = filter.StartDate?.ToString("O"),
                end_date = filter.EndDate?.ToString("O")
            }
        }, JsonOptions);
    }

    private static int? ParseStatus(string status)
    {
        return status.ToLowerInvariant() switch
        {
            "new" => (int)LitterReportStatusEnum.New,
            "assigned" => (int)LitterReportStatusEnum.Assigned,
            "cleaned" => (int)LitterReportStatusEnum.Cleaned,
            "all" => null,
            _ => (int)LitterReportStatusEnum.New
        };
    }

    private static DateTimeOffset? ParseDate(string? dateString)
    {
        if (string.IsNullOrWhiteSpace(dateString))
            return null;

        if (DateTimeOffset.TryParse(dateString, out var date))
            return date;

        return null;
    }

    private static LitterReportDto Sanitize(LitterReport r)
    {
        var statusName = r.LitterReportStatusId switch
        {
            (int)LitterReportStatusEnum.New => "New",
            (int)LitterReportStatusEnum.Assigned => "Assigned",
            (int)LitterReportStatusEnum.Cleaned => "Cleaned",
            (int)LitterReportStatusEnum.Cancelled => "Cancelled",
            _ => "Unknown"
        };

        // Get location from first litter image (location is stored on images, not the report)
        var firstImage = r.LitterImages?.FirstOrDefault(i => !i.IsCancelled);

        return new LitterReportDto
        {
            Id = r.Id,
            Description = r.Description,
            City = firstImage?.City ?? string.Empty,
            Region = firstImage?.Region ?? string.Empty,
            Country = firstImage?.Country ?? string.Empty,
            Status = statusName,
            CreatedDate = r.CreatedDate ?? DateTimeOffset.MinValue,
            Url = $"https://trashmob.eco/litterreport/{r.Id}"
        };
    }
}
