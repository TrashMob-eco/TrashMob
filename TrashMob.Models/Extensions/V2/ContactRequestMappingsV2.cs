namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for contact requests.
    /// </summary>
    public static class ContactRequestMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="ContactRequest"/> entity to a <see cref="ContactRequestDto"/>.
        /// </summary>
        public static ContactRequestDto ToV2Dto(this ContactRequest entity)
        {
            return new ContactRequestDto
            {
                Name = entity.Name,
                Email = entity.Email,
                Message = entity.Message,
            };
        }

        /// <summary>
        /// Maps a <see cref="ContactRequestDto"/> to a <see cref="ContactRequest"/> entity.
        /// </summary>
        public static ContactRequest ToEntity(this ContactRequestDto dto)
        {
            return new ContactRequest
            {
                Name = dto.Name,
                Email = dto.Email,
                Message = dto.Message,
            };
        }
    }
}
