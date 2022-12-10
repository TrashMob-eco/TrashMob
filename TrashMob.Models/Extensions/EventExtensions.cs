namespace TrashMob.Models.Extensions
{
    using System.Text;
    using TrashMob.Models;

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
    }
}
