#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping TeamMember entities to V2 DTOs.
    /// </summary>
    public static class TeamMemberMappingsV2
    {
        /// <summary>
        /// Maps a TeamMember entity to a V2 TeamMemberDto.
        /// Requires the User navigation property to be loaded.
        /// </summary>
        public static TeamMemberDto ToV2Dto(this TeamMember entity)
        {
            return new TeamMemberDto
            {
                Id = entity.Id,
                TeamId = entity.TeamId,
                UserId = entity.UserId,
                UserName = entity.User?.UserName ?? string.Empty,
                GivenName = entity.User?.GivenName ?? string.Empty,
                ProfilePhotoUrl = entity.User?.ProfilePhotoUrl ?? string.Empty,
                IsTeamLead = entity.IsTeamLead,
                JoinedDate = entity.JoinedDate,
            };
        }
    }
}
