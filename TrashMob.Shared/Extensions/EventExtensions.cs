namespace TrashMob.Shared.Extensions
{
    using System;
    using System.Threading.Tasks;
    using TrashMob.Models;
    using TrashMob.Shared.Managers.Interfaces;

    /// <summary>
    /// Extension methods for the Event class.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Gets the local date and time for an event based on its geographic coordinates.
        /// </summary>
        /// <param name="mobEvent">The event to get local time for.</param>
        /// <param name="mapRepository">The map manager for timezone lookups.</param>
        /// <returns>A tuple containing the formatted local date and time strings.</returns>
        public static async Task<Tuple<string, string>> GetLocalEventTime(this Event mobEvent,
            IMapManager mapRepository)
        {
            // Note that the UI should never be passing in a null latitude and longitude... but during testing, this is possible. This hack is bad and will have unintented consequences
            var localTime = await mapRepository
                .GetTimeForPointAsync(new Tuple<double, double>(mobEvent.Latitude ?? 0, mobEvent.Longitude ?? 0),
                    mobEvent.EventDate).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(localTime))
            {
                return new Tuple<string, string>(mobEvent.EventDate.ToString("MMMM dd, yyyy"),
                    mobEvent.EventDate.ToString("h:mm tt"));
            }

            var localDate = DateTimeOffset.Parse(localTime);
            var retVal = new Tuple<string, string>(localDate.ToString("MMMM dd, yyyy"), localDate.ToString("h:mm tt"));
            return retVal;
        }

        /// <summary>
        /// Generates a Google Maps URL for the event location.
        /// </summary>
        /// <param name="mobEvent">The event to generate the URL for.</param>
        /// <returns>A Google Maps URL string for the event's address.</returns>
        public static string GoogleMapsUrl(this Event mobEvent)
        {
            return
                $"https://google.com/maps/place/{mobEvent.StreetAddress}+{mobEvent.City}+{mobEvent.Region}+{mobEvent.PostalCode}+{mobEvent.Country}";
        }

        /// <summary>
        /// Generates the TrashMob event details page URL.
        /// </summary>
        /// <param name="mobEvent">The event to generate the URL for.</param>
        /// <returns>The URL to the event details page on trashmob.eco.</returns>
        public static string EventDetailsUrl(this Event mobEvent)
        {
            // Todo - make this variable depending on environment
            return $"https://www.trashmob.eco/eventdetails/{mobEvent.Id}";
        }

        /// <summary>
        /// Generates the TrashMob event summary page URL.
        /// </summary>
        /// <param name="mobEvent">The event to generate the URL for.</param>
        /// <returns>The URL to the event summary page on trashmob.eco.</returns>
        public static string EventSummaryUrl(this Event mobEvent)
        {
            // Todo - make this variable depending on environment
            return $"https://www.trashmob.eco/eventsummary/{mobEvent.Id}";
        }
    }
}