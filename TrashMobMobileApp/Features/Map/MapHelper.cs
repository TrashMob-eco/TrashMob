namespace TrashMobMobileApp.Features.Map
{
    using Microsoft.Maui.Controls.Maps;
    using TrashMob.Models;

    internal static class MapHelper
    {
        public static Pin GetPinForEvent(Event mobEvent)
        {
            var eventHeader = string.Format("{0}: {1:yyyy-MM-dd HH:mm}", mobEvent.Name, mobEvent.EventDate);

            var pin = new Pin
            {
                Label = eventHeader,
                Address = mobEvent.StreetAddress,
                Type = PinType.Place
            };

            return pin;
        }
    }
}
