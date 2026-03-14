namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for UserFeedback.
    /// </summary>
    public static class UserFeedbackMappingsV2
    {
        /// <summary>
        /// Maps a UserFeedback entity to a UserFeedbackDto.
        /// </summary>
        public static UserFeedbackDto ToV2Dto(this UserFeedback entity)
        {
            return new UserFeedbackDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                Category = entity.Category,
                Description = entity.Description,
                Email = entity.Email,
                ScreenshotUrl = entity.ScreenshotUrl,
                PageUrl = entity.PageUrl,
                UserAgent = entity.UserAgent,
                Status = entity.Status,
                InternalNotes = entity.InternalNotes,
                ReviewedByUserId = entity.ReviewedByUserId,
                ReviewedDate = entity.ReviewedDate,
                GitHubIssueUrl = entity.GitHubIssueUrl,
                CreatedDate = entity.CreatedDate,
            };
        }

        /// <summary>
        /// Maps a UserFeedbackWriteDto to a UserFeedback entity.
        /// </summary>
        public static UserFeedback ToEntity(this UserFeedbackWriteDto dto)
        {
            return new UserFeedback
            {
                Category = dto.Category,
                Description = dto.Description,
                Email = dto.Email,
                ScreenshotUrl = dto.ScreenshotUrl,
                PageUrl = dto.PageUrl,
                UserAgent = dto.UserAgent,
                Status = "New",
            };
        }
    }
}
