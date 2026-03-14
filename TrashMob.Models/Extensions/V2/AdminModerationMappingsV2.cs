namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping admin and moderation entities to V2 DTOs.
    /// </summary>
    public static class AdminModerationMappingsV2
    {
        #region CommunityWaiver

        /// <summary>
        /// Maps a CommunityWaiver entity to a V2 CommunityWaiverDto.
        /// </summary>
        public static CommunityWaiverDto ToV2Dto(this CommunityWaiver entity)
        {
            return new CommunityWaiverDto
            {
                Id = entity.Id,
                CommunityId = entity.CommunityId,
                WaiverVersionId = entity.WaiverVersionId,
                IsRequired = entity.IsRequired,
                CreatedDate = entity.CreatedDate,
            };
        }

        #endregion

        #region MessageRequest

        /// <summary>
        /// Maps a MessageRequest entity to a V2 MessageRequestDto.
        /// </summary>
        public static MessageRequestDto ToV2Dto(this MessageRequest entity)
        {
            return new MessageRequestDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Message = entity.Message,
            };
        }

        /// <summary>
        /// Maps a V2 MessageRequestDto to a MessageRequest entity.
        /// </summary>
        public static MessageRequest ToEntity(this MessageRequestDto dto)
        {
            return new MessageRequest
            {
                Id = dto.Id,
                Name = dto.Name ?? string.Empty,
                Message = dto.Message ?? string.Empty,
            };
        }

        #endregion

        #region PhotoFlag

        /// <summary>
        /// Maps a PhotoFlag entity to a V2 PhotoFlagDto.
        /// </summary>
        public static PhotoFlagDto ToV2Dto(this PhotoFlag entity)
        {
            return new PhotoFlagDto
            {
                Id = entity.Id,
                PhotoId = entity.PhotoId,
                PhotoType = entity.PhotoType,
                FlaggedByUserId = entity.FlaggedByUserId,
                FlagReason = entity.FlagReason,
                FlaggedDate = entity.FlaggedDate,
                ResolvedDate = entity.ResolvedDate,
                ResolvedByUserId = entity.ResolvedByUserId,
                Resolution = entity.Resolution,
            };
        }

        #endregion
    }
}
