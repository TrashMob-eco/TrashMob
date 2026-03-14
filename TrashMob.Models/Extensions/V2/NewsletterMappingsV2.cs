namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for newsletter-related entities.
    /// </summary>
    public static class NewsletterMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="NewsletterCategory"/> to a <see cref="NewsletterCategoryDto"/>.
        /// </summary>
        public static NewsletterCategoryDto ToV2Dto(this NewsletterCategory entity)
        {
            return new NewsletterCategoryDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                IsDefault = entity.IsDefault,
            };
        }

        /// <summary>
        /// Maps a <see cref="Newsletter"/> to a <see cref="NewsletterDto"/>.
        /// </summary>
        public static NewsletterDto ToV2Dto(this Newsletter entity)
        {
            return new NewsletterDto
            {
                Id = entity.Id,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.Name ?? string.Empty,
                Subject = entity.Subject ?? string.Empty,
                PreviewText = entity.PreviewText ?? string.Empty,
                HtmlContent = entity.HtmlContent ?? string.Empty,
                TextContent = entity.TextContent ?? string.Empty,
                TargetType = entity.TargetType ?? string.Empty,
                TargetId = entity.TargetId,
                Status = entity.Status ?? string.Empty,
                ScheduledDate = entity.ScheduledDate,
                SentDate = entity.SentDate,
                RecipientCount = entity.RecipientCount,
                SentCount = entity.SentCount,
                DeliveredCount = entity.DeliveredCount,
                OpenCount = entity.OpenCount,
                ClickCount = entity.ClickCount,
                BounceCount = entity.BounceCount,
                UnsubscribeCount = entity.UnsubscribeCount,
                CreatedDate = entity.CreatedDate ?? DateTimeOffset.MinValue,
                LastUpdatedDate = entity.LastUpdatedDate ?? DateTimeOffset.MinValue,
            };
        }

        /// <summary>
        /// Maps a <see cref="NewsletterTemplate"/> to a <see cref="NewsletterTemplateDto"/>.
        /// </summary>
        public static NewsletterTemplateDto ToV2Dto(this NewsletterTemplate entity)
        {
            return new NewsletterTemplateDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                HtmlContent = entity.HtmlContent ?? string.Empty,
                TextContent = entity.TextContent ?? string.Empty,
                ThumbnailUrl = entity.ThumbnailUrl,
            };
        }
    }
}
