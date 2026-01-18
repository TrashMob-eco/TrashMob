namespace TrashMob.Models.Extensions
{
    using System.Text;

    /// <summary>
    /// Provides extension methods for the Event class.
    /// </summary>
    public static class EventExtensions
    {
        /// <summary>
        /// Formats the event's address components into a single formatted address string.
        /// </summary>
        /// <param name="mobEvent">The event to get the address from.</param>
        /// <returns>A formatted address string combining street address, city, region, postal code, and country.</returns>
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
    }
}