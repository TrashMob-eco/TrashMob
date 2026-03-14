#nullable enable

namespace TrashMob.Models.Extensions.V2
{
    using System.Collections.Generic;
    using System.Linq;
    using TrashMob.Models.Poco;
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
                EventVisibilityId = entity.EventVisibilityId,
                CreatedByUserId = entity.CreatedByUserId,
                CreatedDate = entity.CreatedDate.GetValueOrDefault(),
                LastUpdatedByUserId = entity.LastUpdatedByUserId,
                LastUpdatedDate = entity.LastUpdatedDate.GetValueOrDefault(),
                TeamId = entity.TeamId,
                CreatedByUserName = entity.CreatedByUser?.UserName ?? string.Empty,
            };
        }
        /// <summary>
        /// Maps a V2 EventDto to an Event entity.
        /// </summary>
        public static Event ToEntity(this EventDto dto)
        {
            return new Event
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                EventDate = dto.EventDate,
                DurationHours = dto.DurationHours,
                DurationMinutes = dto.DurationMinutes,
                EventTypeId = dto.EventTypeId,
                EventStatusId = dto.EventStatusId,
                StreetAddress = dto.StreetAddress,
                City = dto.City,
                Region = dto.Region,
                Country = dto.Country,
                PostalCode = dto.PostalCode,
                Latitude = dto.Latitude,
                Longitude = dto.Longitude,
                MaxNumberOfParticipants = dto.MaxNumberOfParticipants,
                EventVisibilityId = dto.EventVisibilityId != 0 ? dto.EventVisibilityId : (dto.IsEventPublic ? (int)EventVisibilityEnum.Public : (int)EventVisibilityEnum.Private),
                CreatedByUserId = dto.CreatedByUserId,
                TeamId = dto.TeamId,
            };
        }

    }
}
