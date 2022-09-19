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

        public static async Task<Tuple<string, string>> GetLocalEventTime(this Event mobEvent, IMapRepository mapRepository)
        {
            var localTime = await mapRepository.GetTimeForPoint(new Tuple<double, double>(mobEvent.Latitude.Value, mobEvent.Longitude.Value), mobEvent.EventDate).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(localTime))
            {
                return new Tuple<string, string>(mobEvent.EventDate.ToString("MMMM dd, yyyy"), mobEvent.EventDate.ToString("h:mm tt"));
            }

            var localDate = DateTimeOffset.Parse(localTime);
            var retVal = new Tuple<string, string>(localDate.ToString("MMMM dd, yyyy"), localDate.ToString("h:mm tt"));
            return retVal;
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
