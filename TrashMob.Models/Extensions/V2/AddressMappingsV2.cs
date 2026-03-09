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
    }
}
