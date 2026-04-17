namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Prospects;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for updating an existing community prospect in the pipeline.
/// </summary>
[McpServerToolType]
public class UpdateProspectTool(ICommunityProspectManager prospectManager)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly string[] StageNames =
    [
        "New", "Contacted", "Responded", "Interested", "Onboarding", "Active", "Declined"
    ];

    /// <summary>
    /// Update an existing community prospect in the TrashMob pipeline.
    /// </summary>
    /// <param name="id">The prospect ID (GUID) to update</param>
    /// <param name="name">Updated organization name</param>
    /// <param name="type">Updated type: Municipality, Nonprofit, HOA, CivicOrg, or Other</param>
    /// <param name="city">Updated city</param>
    /// <param name="region">Updated state or region</param>
    /// <param name="country">Updated country</param>
    /// <param name="population">Updated population served</param>
    /// <param name="website">Updated website URL</param>
    /// <param name="contactName">Updated contact person's full name</param>
    /// <param name="contactEmail">Updated contact email address</param>
    /// <param name="contactTitle">Updated contact job title</param>
    /// <param name="contactPhone">Updated contact phone number</param>
    /// <param name="latitude">Updated latitude</param>
    /// <param name="longitude">Updated longitude</param>
    /// <param name="notes">Updated notes (replaces existing notes)</param>
    /// <param name="pipelineStage">Updated pipeline stage: 0=New, 1=Contacted, 2=Responded, 3=Interested, 4=Onboarding, 5=Active, 6=Declined</param>
    /// <returns>JSON with the updated prospect record</returns>
    [McpServerTool]
    [Description("Update an existing community prospect in the TrashMob pipeline. Only provided fields are updated — omitted fields keep their current values. Use this to add contact info, update notes, or advance the pipeline stage.")]
    public async Task<string> UpdateProspect(
        Guid id,
        string? name = null,
        string? type = null,
        string? city = null,
        string? region = null,
        string? country = null,
        int? population = null,
        string? website = null,
        string? contactName = null,
        string? contactEmail = null,
        string? contactTitle = null,
        string? contactPhone = null,
        double? latitude = null,
        double? longitude = null,
        string? notes = null,
        int? pipelineStage = null)
    {
        var prospect = await prospectManager.GetAsync(id, CancellationToken.None);

        if (prospect is null)
        {
            return JsonSerializer.Serialize(new
            {
                error = $"Prospect with ID '{id}' not found.",
            }, JsonOptions);
        }

        // Only update fields that were explicitly provided
        if (name is not null) prospect.Name = name;
        if (type is not null) prospect.Type = type;
        if (city is not null) prospect.City = city;
        if (region is not null) prospect.Region = region;
        if (country is not null) prospect.Country = country;
        if (population.HasValue) prospect.Population = population.Value;
        if (website is not null) prospect.Website = website;
        if (contactName is not null) prospect.ContactName = contactName;
        if (contactEmail is not null) prospect.ContactEmail = contactEmail;
        if (contactTitle is not null) prospect.ContactTitle = contactTitle;
        if (contactPhone is not null) prospect.ContactPhone = contactPhone;
        if (latitude.HasValue) prospect.Latitude = latitude.Value;
        if (longitude.HasValue) prospect.Longitude = longitude.Value;
        if (notes is not null) prospect.Notes = notes;
        if (pipelineStage.HasValue) prospect.PipelineStage = pipelineStage.Value;

        prospect.LastUpdatedDate = DateTimeOffset.UtcNow;

        var updated = await prospectManager.UpdateAsync(prospect, CancellationToken.None);

        return JsonSerializer.Serialize(new
        {
            message = $"Prospect '{updated.Name}' updated.",
            prospect = ToDto(updated),
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
