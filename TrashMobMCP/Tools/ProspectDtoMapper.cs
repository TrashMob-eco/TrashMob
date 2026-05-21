namespace TrashMobMCP.Tools;

using TrashMob.Models;
using TrashMobMCP.Dtos;

/// <summary>
/// Maps a CommunityProspect + its primary ProspectContact to the MCP-facing ProspectDto.
/// </summary>
internal static class ProspectDtoMapper
{
    public static ProspectDto ToDto(CommunityProspect p, ProspectContact? primary, string[] stageNames)
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
            ContactName = primary?.Name,
            ContactEmail = primary?.Email,
            ContactTitle = primary?.Title,
            PipelineStage = p.PipelineStage,
            PipelineStageName = p.PipelineStage >= 0 && p.PipelineStage < stageNames.Length
                ? stageNames[p.PipelineStage]
                : "Unknown",
            FitScore = p.FitScore,
            LastContactedDate = p.LastContactedDate,
            NextFollowUpDate = p.NextFollowUpDate,
            Notes = p.Notes,
        };
    }
}
