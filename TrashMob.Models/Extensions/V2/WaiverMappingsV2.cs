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
    }
}
