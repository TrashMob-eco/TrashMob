namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for waivers.
    /// </summary>
    public static class WaiverMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="WaiverVersion"/> to a <see cref="WaiverVersionDto"/>.
        /// </summary>
        public static WaiverVersionDto ToV2Dto(this WaiverVersion entity)
        {
            return new WaiverVersionDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Version = entity.Version ?? string.Empty,
                WaiverText = entity.WaiverText ?? string.Empty,
                EffectiveDate = entity.EffectiveDate,
                ExpiryDate = entity.ExpiryDate,
                IsActive = entity.IsActive,
                Scope = entity.Scope,
            };
        }

        /// <summary>
        /// Maps a <see cref="UserWaiver"/> to a <see cref="UserWaiverDto"/>.
        /// </summary>
        public static UserWaiverDto ToV2Dto(this UserWaiver entity)
        {
            return new UserWaiverDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                WaiverVersionId = entity.WaiverVersionId,
                AcceptedDate = entity.AcceptedDate,
                ExpiryDate = entity.ExpiryDate,
                TypedLegalName = entity.TypedLegalName ?? string.Empty,
                SigningMethod = entity.SigningMethod ?? string.Empty,
                DocumentUrl = entity.DocumentUrl ?? string.Empty,
                IsMinor = entity.IsMinor,
                GuardianName = entity.GuardianName ?? string.Empty,
                GuardianRelationship = entity.GuardianRelationship ?? string.Empty,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="WaiverVersionDto"/> back to a <see cref="WaiverVersion"/> entity.
        /// </summary>
        public static WaiverVersion ToEntity(this WaiverVersionDto dto)
        {
            return new WaiverVersion
            {
                Id = dto.Id,
                Name = dto.Name,
                Version = dto.Version,
                WaiverText = dto.WaiverText,
                EffectiveDate = dto.EffectiveDate,
                ExpiryDate = dto.ExpiryDate,
                IsActive = dto.IsActive,
                Scope = dto.Scope,
            };
        }

        /// <summary>
        /// Maps a <see cref="Waiver"/> to a <see cref="WaiverDto"/>.
        /// </summary>
        public static WaiverDto ToV2Dto(this Waiver entity)
        {
            return new WaiverDto
            {
                Id = entity.Id,
                Name = entity.Name,
                IsWaiverEnabled = entity.IsWaiverEnabled,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="WaiverDto"/> back to a <see cref="Waiver"/> entity.
        /// </summary>
        public static Waiver ToEntity(this WaiverDto dto)
        {
            return new Waiver
            {
                Id = dto.Id,
                Name = dto.Name,
                IsWaiverEnabled = dto.IsWaiverEnabled,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="UserWaiverDto"/> back to a <see cref="UserWaiver"/> entity.
        /// </summary>
        public static UserWaiver ToEntity(this UserWaiverDto dto)
        {
            return new UserWaiver
            {
                Id = dto.Id,
                UserId = dto.UserId,
                WaiverVersionId = dto.WaiverVersionId,
                AcceptedDate = dto.AcceptedDate,
                ExpiryDate = dto.ExpiryDate,
                TypedLegalName = dto.TypedLegalName,
                SigningMethod = dto.SigningMethod,
                DocumentUrl = dto.DocumentUrl,
                IsMinor = dto.IsMinor,
                GuardianName = dto.GuardianName,
                GuardianRelationship = dto.GuardianRelationship,
            };
        }
    }
}
