namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Interfaces;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching partner locations that provide services to cleanup events.
/// </summary>
[McpServerToolType]
public class SearchPartnerLocationsTool
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };
    private readonly IKeyedManager<PartnerLocation> _partnerLocationManager;

    public SearchPartnerLocationsTool(IKeyedManager<PartnerLocation> partnerLocationManager)
    {
        _partnerLocationManager = partnerLocationManager;
    }

    /// <summary>
    /// Search for partner locations that provide services to cleanup events.
    /// </summary>
    /// <param name="city">City to search in (optional)</param>
    /// <param name="region">State/region to search in (optional)</param>
    /// <param name="country">Country to search in (optional, defaults to "United States")</param>
    /// <param name="serviceType">Filter by service type: "hauling", "disposal", "supplies", "kits", or "all" (default: "all")</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 50)</param>
    /// <returns>JSON array of partner locations with their services</returns>
    [McpServerTool]
    [Description("Search for partner locations that provide services to TrashMob cleanup events. Services include: hauling (pickup of collected trash), disposal (dumpster locations), supplies (gloves, bags, etc.), and startup kits for new events.")]
    public async Task<string> SearchPartnerLocations(
        string? city = null,
        string? region = null,
        string? country = null,
        string serviceType = "all",
        int maxResults = 20)
    {
        var locations = await _partnerLocationManager.GetAsync(
            pl => pl.IsActive &&
                  (string.IsNullOrEmpty(city) || pl.City == city) &&
                  (string.IsNullOrEmpty(region) || pl.Region == region) &&
                  (string.IsNullOrEmpty(country) || pl.Country == country || country == "United States"),
            CancellationToken.None);

        // Filter by service type if specified
        var serviceTypeId = ParseServiceType(serviceType);
        if (serviceTypeId.HasValue)
        {
            locations = locations.Where(pl =>
                pl.PartnerLocationServices?.Any(s => s.ServiceTypeId == serviceTypeId.Value) == true);
        }

        var sanitizedLocations = locations
            .Take(Math.Min(maxResults, 50))
            .Select(Sanitize)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            partner_locations = sanitizedLocations,
            total_count = sanitizedLocations.Count,
            search_criteria = new
            {
                city,
                region,
                country = country ?? "United States",
                service_type = serviceType
            }
        }, JsonOptions);
    }

    private static int? ParseServiceType(string serviceType)
    {
        return serviceType.ToLowerInvariant() switch
        {
            "hauling" => (int)ServiceTypeEnum.Hauling,
            "disposal" => (int)ServiceTypeEnum.DisposalLocation,
            "supplies" => (int)ServiceTypeEnum.Supplies,
            "kits" or "startupkits" => (int)ServiceTypeEnum.StartupKits,
            "all" => null,
            _ => null
        };
    }

    private static PartnerLocationDto Sanitize(PartnerLocation pl)
    {
        var services = pl.PartnerLocationServices?
            .Select(s => GetServiceTypeName(s.ServiceTypeId))
            .Where(s => s != null)
            .Cast<string>()
            .ToList() ?? [];

        return new PartnerLocationDto
        {
            Id = pl.Id,
            Name = pl.Name,
            PartnerName = pl.Partner?.Name ?? string.Empty,
            City = pl.City,
            Region = pl.Region,
            Country = pl.Country,
            PublicNotes = pl.PublicNotes,
            Services = services,
            IsActive = pl.IsActive
        };
    }

    private static string? GetServiceTypeName(int serviceTypeId)
    {
        return serviceTypeId switch
        {
            (int)ServiceTypeEnum.Hauling => "Hauling",
            (int)ServiceTypeEnum.DisposalLocation => "Disposal Location",
            (int)ServiceTypeEnum.Supplies => "Supplies",
            (int)ServiceTypeEnum.StartupKits => "Startup Kits",
            _ => null
        };
    }
}
