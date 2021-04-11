namespace TrashMob.Extensions
{
    using System;
    using TrashMob.Models;

    public static class EventExtensions
    {
        public static EventHistory ToEventHistory(this Event originalEvent)
        {
            return new EventHistory
            {
                Id = Guid.NewGuid(),
                EventId = originalEvent.Id,
                Name = originalEvent.Name,
                Description = originalEvent.Description,
                EventDate = originalEvent.EventDate,
                EventTypeId = originalEvent.EventTypeId,
                EventStatusId = originalEvent.EventStatusId,
                StreetAddress = originalEvent.StreetAddress,
                City = originalEvent.City,
                StateProvince = originalEvent.StateProvince,
                Country = originalEvent.Country,
                ZipCode = originalEvent.ZipCode,
                Latitude = originalEvent.Latitude,
                Longitude = originalEvent.Longitude,
                Gpscoords = originalEvent.Gpscoords,
                MaxNumberOfParticipants = originalEvent.MaxNumberOfParticipants,
                ActualNumberOfParticipants = originalEvent.ActualNumberOfParticipants,
                CreatedByUserId = originalEvent.CreatedByUserId,
                CreatedDate = originalEvent.CreatedDate,
                LastUpdatedByUserId = originalEvent.LastUpdatedByUserId,
                LastUpdatedDate = originalEvent.LastUpdatedDate,
            };
        }
    }
}
