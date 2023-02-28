namespace TrashMobMobileApp.Features.Map
{
    using Microsoft.Maui.Controls.Maps;
    using TrashMob.Models;
    using TrashMobMobileApp.Extensions;

    internal static class MapHelper
    {
        public static Pin GetPinForEvent(Event mobEvent)
        {
            var eventHeader = string.Format("{0}: {1} {2}", mobEvent.Name, mobEvent.EventDate.GetFormattedLocalDate(), mobEvent.EventDate.GetFormattedLocalTime());
            var eventLocation = string.Format("{0}, {1}, {2}", mobEvent.StreetAddress, mobEvent.City, mobEvent.Region);
            var pin = new TrashMobPin
            {
                Label = eventHeader,
                Address = eventLocation,
                Type = PinType.Place,
                EventId = mobEvent.Id,                
            };

            return pin;
        }

        public static Pin GetPinForUser(User user)
        {
            var userHeader = user.UserName;
            var address = string.Format("{0}, {1}", user.City, user.Region);
            var pin = new Pin
            {
                Label = userHeader,
                Address = address,
                Type = PinType.Place
            };

            return pin;
        }
    }
}
