namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for parental consent records.
    /// </summary>
    public static class ParentalConsentMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="ParentalConsent"/> entity to a <see cref="ParentalConsentDto"/>.
        /// </summary>
        public static ParentalConsentDto ToV2Dto(this ParentalConsent entity)
        {
            return new ParentalConsentDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                ParentUserId = entity.ParentUserId,
                DependentId = entity.DependentId,
                ConsentType = (int)entity.ConsentType,
                Status = (int)entity.Status,
                ConsentUrl = entity.ConsentUrl ?? string.Empty,
                VerifiedDate = entity.VerifiedDate,
                CreatedDate = entity.CreatedDate,
            };
        }
    }
}
