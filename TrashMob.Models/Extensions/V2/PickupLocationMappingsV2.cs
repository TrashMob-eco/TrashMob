namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for pickup locations.
    /// </summary>
    public static class PickupLocationMappingsV2
    {
        /// <summary>
        /// Maps a <see cref="PickupLocation"/> to a <see cref="PickupLocationDto"/>.
        /// </summary>
        public static PickupLocationDto ToV2Dto(this PickupLocation entity)
        {
            return new PickupLocationDto
            {
                Id = entity.Id,
                EventId = entity.EventId,
                Name = entity.Name ?? string.Empty,
                StreetAddress = entity.StreetAddress ?? string.Empty,
                City = entity.City ?? string.Empty,
                Region = entity.Region ?? string.Empty,
                PostalCode = entity.PostalCode ?? string.Empty,
                Country = entity.Country ?? string.Empty,
                County = entity.County ?? string.Empty,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                HasBeenSubmitted = entity.HasBeenSubmitted,
                HasBeenPickedUp = entity.HasBeenPickedUp,
                Notes = entity.Notes ?? string.Empty,
            };
        }

        /// <summary>
        /// Maps a <see cref="PickupLocationDto"/> to a <see cref="PickupLocation"/> entity.
        /// </summary>
        public static PickupLocation ToEntity(this PickupLocationDto dto)
        {
            return new PickupLocation
            {
                Id = dto.Id,
                EventId = dto.EventId,
                Name = dto.Name,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                County = dto.County,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                HasBeenSubmitted = dto.HasBeenSubmitted,
                HasBeenPickedUp = dto.HasBeenPickedUp,
                Notes = dto.Notes,
            };
        }
    }
}
