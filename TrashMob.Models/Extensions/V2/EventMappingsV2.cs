#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for mapping Event entities to V2 DTOs.
    /// </summary>
    public static class EventMappingsV2
    {
        /// <summary>
        /// Maps an Event entity to a V2 EventDto.
        /// </summary>
        public static EventDto ToV2Dto(this Event entity)
        {
            return new EventDto
            {
                Id = entity.Id,
                Name = entity.Name,
                Description = entity.Description,
                EventDate = entity.EventDate,
                DurationHours = entity.DurationHours,
                DurationMinutes = entity.DurationMinutes,
                EventTypeId = entity.EventTypeId,
                EventStatusId = entity.EventStatusId,
                StreetAddress = entity.StreetAddress,
                City = entity.City,
                Region = entity.Region,
                Country = entity.Country,
                PostalCode = entity.PostalCode,
                Latitude = entity.Latitude,
                Longitude = entity.Longitude,
                MaxNumberOfParticipants = entity.MaxNumberOfParticipants,
                IsEventPublic = entity.EventVisibilityId == (int)EventVisibilityEnum.Public,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
            };
        }
    }
}
