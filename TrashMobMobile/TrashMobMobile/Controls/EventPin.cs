namespace TrashMobMobile.Controls
{
    using System;
    using Xamarin.Forms.Maps;

    public class EventPin : Pin
    {
        public Guid EventId { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime EventDate { get; set; }
    }
}
