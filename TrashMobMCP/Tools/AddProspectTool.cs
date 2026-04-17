namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Prospects;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for adding a community prospect directly to the pipeline.
/// </summary>
[McpServerToolType]
public class AddProspectTool(ICommunityProspectManager prospectManager)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly string[] StageNames =
    [
        "New", "Contacted", "Responded", "Interested", "Onboarding", "Active", "Declined"
    ];

    /// <summary>
    /// Add a new community prospect to the TrashMob pipeline.
    /// </summary>
    /// <param name="name">Organization or municipality name (required)</param>
    /// <param name="type">Prospect type: Municipality, Nonprofit, HOA, CivicOrg, or Other (required)</param>
    /// <param name="city">City (required)</param>
    /// <param name="region">State or region (required)</param>
    /// <param name="country">Country (default: United States)</param>
    /// <param name="population">Estimated population served</param>
    /// <param name="website">Organization website URL</param>
    /// <param name="contactName">Best contact person's full name</param>
    /// <param name="contactEmail">Contact person's email address</param>
    /// <param name="contactTitle">Contact person's job title (e.g., "Public Works Director")</param>
    /// <param name="contactPhone">Contact person's phone number</param>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="notes">Research notes or rationale for why this is a good prospect</param>
    /// <returns>JSON with the created prospect record</returns>
    [McpServerTool]
    [Description("Add a new community prospect to the TrashMob pipeline. Use this after researching an organization to save it as a prospect for outreach. Provide as much detail as available — name, type, city, and region are required.")]
    public async Task<string> AddProspect(
        string name,
        string type,
        string city,
        string region,
        string country = "United States",
        int? population = null,
        string? website = null,
        string? contactName = null,
        string? contactEmail = null,
        string? contactTitle = null,
        string? contactPhone = null,
        double? latitude = null,
        double? longitude = null,
        string? notes = null)
    {
        var prospect = new CommunityProspect
        {
            Name = name,
            Type = type,
            City = city,
            Region = region,
            Country = country,
            Population = population,
            Website = website,
            ContactName = contactName,
            ContactEmail = contactEmail,
            ContactTitle = contactTitle,
            ContactPhone = contactPhone,
            Latitude = latitude,
            Longitude = longitude,
            Notes = notes,
            PipelineStage = 0, // New
            FitScore = 0,
        };

        var created = await prospectManager.AddAsync(prospect, CancellationToken.None);

        return JsonSerializer.Serialize(new
        {
            message = $"Prospect '{created.Name}' added to pipeline.",
            prospect = ToDto(created),
        }, JsonOptions);
    }

    private static ProspectDto ToDto(CommunityProspect p)
    {
        return new ProspectDto
        {
            Id = p.Id,
            Name = p.Name,
            Type = p.Type,
            City = p.City,
            Region = p.Region,
            Country = p.Country,
            Population = p.Population,
            Website = p.Website,
            ContactName = p.ContactName,
            ContactEmail = p.ContactEmail,
            ContactTitle = p.ContactTitle,
            PipelineStage = p.PipelineStage,
            PipelineStageName = p.PipelineStage >= 0 && p.PipelineStage < StageNames.Length
                ? StageNames[p.PipelineStage]
                : "Unknown",
            FitScore = p.FitScore,
            LastContactedDate = p.LastContactedDate,
            NextFollowUpDate = p.NextFollowUpDate,
            Notes = p.Notes,
        };
    }
}
