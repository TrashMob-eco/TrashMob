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
    /// <param name="contactName">Best contact person's full name (saved as primary contact)</param>
    /// <param name="contactEmail">Contact person's email address</param>
    /// <param name="contactTitle">Contact person's job title (e.g., "Public Works Director")</param>
    /// <param name="contactPhone">Contact person's phone number</param>
    /// <param name="latitude">Latitude coordinate</param>
    /// <param name="longitude">Longitude coordinate</param>
    /// <param name="notes">Research notes or rationale for why this is a good prospect</param>
    /// <returns>JSON with the created prospect record</returns>
    [McpServerTool]
    [Description("Add a new community prospect to the TrashMob pipeline. Use this after researching an organization to save it as a prospect for outreach. Provide as much detail as available — name, type, city, and region are required. Any contact info given is saved as the prospect's primary contact.")]
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
            Latitude = latitude,
            Longitude = longitude,
            Notes = notes,
            PipelineStage = 0, // New
            FitScore = 0,
        };

        var created = await prospectManager.AddAsync(prospect, CancellationToken.None);

        ProspectContact? primary = null;
        if (!string.IsNullOrWhiteSpace(contactName)
            || !string.IsNullOrWhiteSpace(contactEmail)
            || !string.IsNullOrWhiteSpace(contactTitle)
            || !string.IsNullOrWhiteSpace(contactPhone))
        {
            primary = await prospectManager.UpsertPrimaryContactAsync(
                created.Id,
                contactName ?? string.Empty,
                contactEmail ?? string.Empty,
                contactTitle ?? string.Empty,
                contactPhone ?? string.Empty,
                created.CreatedByUserId,
                CancellationToken.None);
        }

        return JsonSerializer.Serialize(new
        {
            message = $"Prospect '{created.Name}' added to pipeline.",
            prospect = ProspectDtoMapper.ToDto(created, primary, StageNames),
        }, JsonOptions);
    }
}
