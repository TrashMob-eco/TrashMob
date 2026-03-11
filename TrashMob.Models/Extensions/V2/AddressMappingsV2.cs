namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for addresses.
    /// </summary>
    public static class AddressMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="Address"/> to an <see cref="AddressDto"/>.
        /// </summary>
        public static AddressDto ToV2Dto(this Address entity)
        {
            return new AddressDto
            {
                StreetAddress = entity.StreetAddress ?? string.Empty,
                City = entity.City ?? string.Empty,
                Region = entity.Region ?? string.Empty,
                PostalCode = entity.PostalCode ?? string.Empty,
                Country = entity.Country ?? string.Empty,
                County = entity.County ?? string.Empty,
            };
        }

        /// <summary>
        /// Maps a V2 <see cref="AddressDto"/> back to an <see cref="Address"/>.
        /// </summary>
        public static Address ToEntity(this AddressDto dto)
        {
            return new Address
            {
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                Region = dto.Region,
                PostalCode = dto.PostalCode,
                Country = dto.Country,
                County = dto.County,
            };
        }
    }
}
