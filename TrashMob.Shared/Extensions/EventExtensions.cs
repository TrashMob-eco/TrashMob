namespace TrashMob.Shared.Extensions
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    public static class EventExtensions
    {      
        public static async Task<Tuple<string, string>> GetLocalEventTime(this Event mobEvent, IMapManager mapRepository)
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
