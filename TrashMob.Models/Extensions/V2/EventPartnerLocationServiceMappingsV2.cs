namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// V2 mapping extensions for event partner location services.
    /// </summary>
    public static class EventPartnerLocationServiceMappingsV2
    {
        /// <summary>
        /// Maps an <see cref="EventPartnerLocationServiceRequestDto"/> to an <see cref="EventPartnerLocationService"/> entity.
        /// </summary>
        public static EventPartnerLocationService ToEntity(this EventPartnerLocationServiceRequestDto dto)
        {
            return new EventPartnerLocationService
            {
                EventId = dto.EventId,
                PartnerLocationId = dto.PartnerLocationId,
                ServiceTypeId = dto.ServiceTypeId,
                EventPartnerLocationServiceStatusId = dto.EventPartnerLocationServiceStatusId,
            };
        }
    }
}
