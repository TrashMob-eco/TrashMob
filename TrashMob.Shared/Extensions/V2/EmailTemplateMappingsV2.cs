namespace TrashMob.Shared.Extensions.V2
{
    using TrashMob.Models.Poco.V2;
    using TrashMob.Shared.Poco;

    /// <summary>
    /// Extension methods for mapping EmailTemplate to V2 DTOs.
    /// </summary>
    public static class EmailTemplateMappingsV2
    {
        /// <summary>
        /// Maps an EmailTemplate to a V2 EmailTemplateDto.
        /// </summary>
        public static EmailTemplateDto ToV2Dto(this EmailTemplate entity)
        {
            return new EmailTemplateDto
            {
                Name = entity.Name,
                Content = entity.Content,
            };
        }
    }
}
