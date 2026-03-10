#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping DependentInvitation entities to V2 DTOs.
    /// </summary>
    public static class DependentInvitationMappingsV2
    {
        /// <summary>
        /// Maps a DependentInvitation entity to a V2 DependentInvitationDto.
        /// Excludes security-sensitive fields (token hash).
        /// </summary>
        public static DependentInvitationDto ToV2Dto(this DependentInvitation entity)
        {
            return new DependentInvitationDto
            {
                Id = entity.Id,
                DependentId = entity.DependentId,
                ParentUserId = entity.ParentUserId,
                Email = entity.Email ?? string.Empty,
                InvitationStatusId = entity.InvitationStatusId,
                DateInvited = entity.DateInvited,
                ExpiresDate = entity.ExpiresDate,
                DateAccepted = entity.DateAccepted,
                AcceptedByUserId = entity.AcceptedByUserId,
            };
        }
    }
}
