namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 DTO mappings for role administration (Project 64 Phase 3).
    /// </summary>
    public static class RoleMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="Role"/> entity to a <see cref="RoleDto"/>. The
        /// <paramref name="memberCount"/> is optional — pass it on list endpoints
        /// so the admin UI can render counts without a second round trip.
        /// </summary>
        public static RoleDto ToV2Dto(this Role entity, int memberCount = 0) => new()
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive,
            MemberCount = memberCount,
        };

        /// <summary>
        /// Maps a <see cref="UserRole"/> grant to a <see cref="UserRoleGrantDto"/>.
        /// Assumes the caller has eager-loaded the <see cref="UserRole.Role"/>
        /// navigation.
        /// </summary>
        public static UserRoleGrantDto ToV2Dto(this UserRole entity) => new()
        {
            Id = entity.Id,
            UserId = entity.UserId,
            RoleId = entity.RoleId,
            RoleName = entity.Role?.Name ?? string.Empty,
            RoleDescription = entity.Role?.Description,
            GrantedByUserId = entity.GrantedByUserId,
            GrantedDate = entity.GrantedDate,
            ExpiryDate = entity.ExpiryDate,
        };
    }
}
