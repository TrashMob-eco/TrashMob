namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping User entities to V2 DTOs.
    /// </summary>
    public static class UserMappingsV2
    {
        /// <summary>
        /// Maps a User entity to a V2 UserDto including all fields needed by the frontend.
        /// </summary>
        /// <param name="entity">The User entity to map.</param>
        /// <returns>A UserDto containing user properties.</returns>
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
                TravelLimitForLocalEvents = entity.TravelLimitForLocalEvents,
                Email = entity.Email,
                IsSiteAdmin = entity.IsSiteAdmin,
                DateAgreedToTrashMobWaiver = entity.DateAgreedToTrashMobWaiver,
                TrashMobWaiverVersion = entity.TrashMobWaiverVersion,
                DateOfBirth = entity.DateOfBirth,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="UserDto"/> back to a <see cref="User"/> entity.
        /// </summary>
        public static User ToEntity(this UserDto dto)
        {
            return new User
            {
                Id = dto.Id,
                UserName = dto.UserName,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                PrefersMetric = dto.PrefersMetric,
                GivenName = dto.GivenName,
                Surname = dto.Surname,
                ProfilePhotoUrl = dto.ProfilePhotoUrl,
                MemberSince = dto.MemberSince,
                IsMinor = dto.IsMinor,
                TravelLimitForLocalEvents = dto.TravelLimitForLocalEvents,
                Email = dto.Email,
                IsSiteAdmin = dto.IsSiteAdmin,
                DateAgreedToTrashMobWaiver = dto.DateAgreedToTrashMobWaiver,
                TrashMobWaiverVersion = dto.TrashMobWaiverVersion,
                DateOfBirth = dto.DateOfBirth,
            };
        }

        /// <summary>
        /// Maps a User entity to a V2 UserWriteDto for create/update operations.
        /// Includes PII fields needed for write endpoints.
        /// </summary>
        public static Poco.V2.UserWriteDto ToWriteDto(this User entity)
        {
            return new Poco.V2.UserWriteDto
            {
                Id = entity.Id,
                UserName = entity.UserName,
                Email = entity.Email,
                GivenName = entity.GivenName,
                Surname = entity.Surname,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PostalCode = entity.PostalCode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                PrefersMetric = entity.PrefersMetric,
                DateOfBirth = entity.DateOfBirth,
                IsMinor = entity.IsMinor,
                TravelLimitForLocalEvents = entity.TravelLimitForLocalEvents,
                ProfilePhotoUrl = entity.ProfilePhotoUrl,
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
                Email = dto.Email ?? string.Empty,
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
