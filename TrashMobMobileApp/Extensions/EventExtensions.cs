namespace TrashMobMobileApp.Extensions
{
    using TrashMob.Models;

    public static class EventExtensions
    {
        public static string GetFormattedAddress(this Event mobEvent)
        {
            if (string.IsNullOrEmpty(mobEvent.StreetAddress) && string.IsNullOrEmpty(mobEvent.City)
                && string.IsNullOrEmpty(mobEvent.Country) && string.IsNullOrEmpty(mobEvent.PostalCode) && string.IsNullOrEmpty(mobEvent.Region))
            {
                return string.Empty;
            }

            return string.Concat(mobEvent.StreetAddress, ", ", mobEvent.City, ", ", mobEvent.Region, ", ", mobEvent.PostalCode, ", ", mobEvent.Country);
        }

        public static string GetFormattedDuration(this Event mobEvent)
        {
            //TODO: move hard code string to resource file
            return string.Concat(mobEvent.DurationHours, " hour ", mobEvent.DurationMinutes, " minutes");
        }

        public static string GetPublicEventText(this Event mobEvent)
        {
            //TODO: move hard code string to resource file
            return mobEvent.IsEventPublic ? "Yes" : "No";
        }

        public static string GetEventStatusText(this Event mobEvent)
        {
            //TODO: move hard code string to resource file
            if (mobEvent.EventStatusId == 1)
            {
                return "Active";
            }
            else if (mobEvent.EventStatusId == 2)
            {
                return "Full";
            }
            else if (mobEvent.EventStatusId == 3)
            {
                return "Cancelled";
            }
            else if (mobEvent.EventStatusId == 4)
            {
                return "Complete";
            }

            return "N/A";
        }
    }
}
