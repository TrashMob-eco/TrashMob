namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for newsletter categories.
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
    }
}
