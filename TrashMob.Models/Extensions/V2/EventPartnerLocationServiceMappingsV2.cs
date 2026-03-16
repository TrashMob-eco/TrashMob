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

        /// <summary>
        /// Maps an <see cref="EventPartnerLocationService"/> entity to a <see cref="EventPartnerLocationServiceDto"/>.
        /// </summary>
        public static EventPartnerLocationServiceDto ToV2Dto(this EventPartnerLocationService entity)
        {
            return new EventPartnerLocationServiceDto
            {
                EventId = entity.EventId,
                PartnerLocationId = entity.PartnerLocationId,
                ServiceTypeId = entity.ServiceTypeId,
                EventPartnerLocationServiceStatusId = entity.EventPartnerLocationServiceStatusId,
            };
        }
    }
}
