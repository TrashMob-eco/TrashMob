namespace TrashMobMobile.Extensions;

using TrashMob.Models;
using TrashMobMobile.ViewModels;

public static class TeamExtensions
{
    public static TeamViewModel ToTeamViewModel(this Team team, int memberCount = 0, bool isUserMember = false)
    {
        var locationParts = new List<string>();
        if (!string.IsNullOrWhiteSpace(team.City))
        {
            locationParts.Add(team.City);
        }

        if (!string.IsNullOrWhiteSpace(team.Region))
        {
            locationParts.Add(team.Region);
        }

        if (!string.IsNullOrWhiteSpace(team.Country))
        {
            locationParts.Add(team.Country);
        }

        return new TeamViewModel
        {
            Id = team.Id,
            Name = team.Name ?? string.Empty,
            Description = team.Description ?? string.Empty,
            Location = string.Join(", ", locationParts),
            IsPublic = team.IsPublic,
            RequiresApproval = team.RequiresApproval,
            MemberCount = memberCount,
            MemberCountDisplay = memberCount == 1 ? "1 member" : $"{memberCount} members",
            IsUserMember = isUserMember,
        };
    }

    public static TeamMemberViewModel ToTeamMemberViewModel(this TeamMember member)
    {
        var userName = member.User?.UserName ?? "Unknown";

        return new TeamMemberViewModel
        {
            UserId = member.UserId,
            UserName = userName,
            IsTeamLead = member.IsTeamLead,
            Role = member.IsTeamLead ? "Lead" : "Member",
            JoinedDate = member.JoinedDate.GetFormattedLocalDate(),
        };
    }
}
