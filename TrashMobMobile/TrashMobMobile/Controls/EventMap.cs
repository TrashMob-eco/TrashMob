namespace TrashMobMobile.Controls
{
    using System.Collections.Generic;
    using Xamarin.Forms.Maps;

    public class EventMap : Map
    {
        public List<EventPin> EventPins { get; set; }
    }
}
