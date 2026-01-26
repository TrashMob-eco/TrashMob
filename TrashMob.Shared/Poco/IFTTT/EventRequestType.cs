namespace TrashMob.Shared.Poco.IFTTT
{
    /// <summary>
    /// Defines the types of event request filters for IFTTT integration.
    /// </summary>
    public enum EventRequestType
    {
        /// <summary>
        /// Retrieves all events without any location filter.
        /// </summary>
        All = 0,

        /// <summary>
        /// Filters events by country.
        /// </summary>
        ByCountry = 1,

        /// <summary>
        /// Filters events by region or state.
        /// </summary>
        ByRegion = 2,

        /// <summary>
        /// Filters events by city.
        /// </summary>
        ByCity = 3,

        /// <summary>
        /// Filters events by postal code.
        /// </summary>
        ByPostalCode = 4,
    }
}
