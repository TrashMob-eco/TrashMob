namespace TrashMobMCP.Tools;

using System.ComponentModel;
using System.Text.Json;
using ModelContextProtocol.Server;
using TrashMob.Models;
using TrashMob.Shared.Managers.Prospects;
using TrashMobMCP.Dtos;

/// <summary>
/// MCP tool for searching existing community prospects in the pipeline.
/// </summary>
[McpServerToolType]
public class SearchProspectsTool(ICommunityProspectManager prospectManager)
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    private static readonly string[] StageNames =
    [
        "New", "Contacted", "Responded", "Interested", "Onboarding", "Active", "Declined"
    ];

    /// <summary>
    /// Search existing community prospects in the TrashMob pipeline.
    /// </summary>
    /// <param name="searchTerm">Search by name, city, or region (optional)</param>
    /// <param name="pipelineStage">Filter by pipeline stage: 0=New, 1=Contacted, 2=Responded, 3=Interested, 4=Onboarding, 5=Active, 6=Declined (optional)</param>
    /// <param name="maxResults">Maximum number of results to return (default: 20, max: 100)</param>
    /// <returns>JSON array of matching prospects with pipeline status</returns>
    [McpServerTool]
    [Description("Search existing community prospects in the TrashMob pipeline. Prospects are organizations being evaluated as potential community partners. Filter by search term, pipeline stage, or both.")]
    public async Task<string> SearchProspects(
        string? searchTerm = null,
        int? pipelineStage = null,
        int maxResults = 20)
    {
        IEnumerable<CommunityProspect> prospects;

        if (pipelineStage.HasValue)
        {
            prospects = await prospectManager.GetByPipelineStageAsync(pipelineStage.Value);
        }
        else if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            prospects = await prospectManager.SearchAsync(searchTerm);
        }
        else
        {
            prospects = await prospectManager.GetAsync(CancellationToken.None);
        }

        var sanitized = prospects
            .Take(Math.Min(maxResults, 100))
            .Select(ToDto)
            .ToList();

        return JsonSerializer.Serialize(new
        {
            prospects = sanitized,
            total_count = sanitized.Count,
            search_criteria = new
            {
                search_term = searchTerm,
                pipeline_stage = pipelineStage,
                pipeline_stage_name = pipelineStage.HasValue && pipelineStage.Value >= 0 && pipelineStage.Value < StageNames.Length
                    ? StageNames[pipelineStage.Value]
                    : null,
            }
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
