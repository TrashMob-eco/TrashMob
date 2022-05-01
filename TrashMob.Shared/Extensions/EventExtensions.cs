namespace TrashMob.Shared.Extensions
{
    using Microsoft.Extensions.Logging;
    using System;
    using System.Text;
    using System.Threading.Tasks;
    using TrashMob.Shared.Models;
    using TrashMob.Shared.Persistence;

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
                DurationHours = originalEvent.DurationHours,
                DurationMinutes = originalEvent.DurationMinutes,
                EventTypeId = originalEvent.EventTypeId,
                EventStatusId = originalEvent.EventStatusId,
                StreetAddress = originalEvent.StreetAddress,
                City = originalEvent.City,
                Region = originalEvent.Region,
                Country = originalEvent.Country,
                PostalCode = originalEvent.PostalCode,
                Latitude = originalEvent.Latitude,
                Longitude = originalEvent.Longitude,
                MaxNumberOfParticipants = originalEvent.MaxNumberOfParticipants,
                IsEventPublic = originalEvent.IsEventPublic,
                CreatedByUserId = originalEvent.CreatedByUserId,
                CreatedDate = originalEvent.CreatedDate,
                LastUpdatedByUserId = originalEvent.LastUpdatedByUserId,
                LastUpdatedDate = originalEvent.LastUpdatedDate,
                CancellationReason = originalEvent.CancellationReason,
            };
        }

        public static string EventAddress(this Event mobEvent)
        {
            var eventAddress = new StringBuilder();

            if (!string.IsNullOrWhiteSpace(mobEvent.StreetAddress))
            {
                eventAddress.Append(mobEvent.StreetAddress);
                eventAddress.Append(", ");
            }

            if (!string.IsNullOrWhiteSpace(mobEvent.City))
            {
                eventAddress.Append(mobEvent.City);
                eventAddress.Append(", ");
            }

            if (!string.IsNullOrWhiteSpace(mobEvent.Region))
            {
                eventAddress.Append(mobEvent.Region);
                eventAddress.Append(',');
            }

            if (!string.IsNullOrWhiteSpace(mobEvent.PostalCode))
            {
                eventAddress.Append(mobEvent.PostalCode);
                eventAddress.Append(' ');
            }

            if (!string.IsNullOrWhiteSpace(mobEvent.Country))
            {
                eventAddress.Append(mobEvent.Country);
            }

            return eventAddress.ToString();
        }

        public static async Task<DateTime> GetLocalEventTime(this Event mobEvent, IMapRepository mapRepository)
        {
            var localTime = await mapRepository.GetTimeForPoint(new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value), mobEvent.EventDate).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(localTime))
            {
                return mobEvent.EventDate.DateTime;
            }
            
            return DateTime.Parse(localTime);
        }

        public static string GoogleMapsUrl(this Event mobEvent)
        {
            return $"https://google.com/maps/place/{mobEvent.StreetAddress}+{mobEvent.City}+{mobEvent.Region}+{mobEvent.PostalCode}+{mobEvent.Country}";
        }

        public static string EventDetailsUrl(this Event mobEvent)
        {
            // Todo - make this variable depending on environment
            return $"https://www.trashmob.eco/eventdetails/{mobEvent.Id}";
        }

        public static string EventSummaryUrl(this Event mobEvent)
        {
            // Todo - make this variable depending on environment
            return $"https://www.trashmob.eco/eventsummary/{mobEvent.Id}";
        }
    }
}
