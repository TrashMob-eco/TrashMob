namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for lookup models.
    /// </summary>
    public static class LookupMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="LookupModel"/> to a <see cref="LookupItemDto"/>.
        /// </summary>
        public static LookupItemDto ToV2Dto(this LookupModel entity)
        {
            return new LookupItemDto
            {
                Id = entity.Id,
                Name = entity.Name ?? string.Empty,
                Description = entity.Description ?? string.Empty,
                DisplayOrder = entity.DisplayOrder ?? 0,
            };
        }
    }
}
