namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping User entities to V2 DTOs.
    /// </summary>
    public static class UserMappingsV2
    {
        /// <summary>
        /// Maps a User entity to a V2 UserDto, excluding PII fields.
        /// </summary>
        /// <param name="entity">The User entity to map.</param>
        /// <returns>A UserDto containing only public-safe properties.</returns>
        public static UserDto ToV2Dto(this User entity)
        {
            return new UserDto
            {
                Id = entity.Id,
                UserName = entity.UserName,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PostalCode = entity.PostalCode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                PrefersMetric = entity.PrefersMetric,
                GivenName = entity.GivenName,
                Surname = entity.Surname,
                ProfilePhotoUrl = entity.ProfilePhotoUrl,
                MemberSince = entity.MemberSince,
                IsMinor = entity.IsMinor,
            };
        }

        /// <summary>
        /// Maps a V2 UserWriteDto to a User entity for create/update operations.
        /// </summary>
        public static User ToEntity(this Poco.V2.UserWriteDto dto)
        {
            return new User
            {
                Id = dto.Id,
                UserName = dto.UserName,
                Email = dto.Email,
                GivenName = dto.GivenName,
                Surname = dto.Surname,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PrefersMetric = dto.PrefersMetric,
                DateOfBirth = dto.DateOfBirth,
                IsMinor = dto.IsMinor,
                TravelLimitForLocalEvents = dto.TravelLimitForLocalEvents,
                ProfilePhotoUrl = dto.ProfilePhotoUrl,
            };
        }
    }
}
