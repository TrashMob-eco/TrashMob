namespace TrashMobMobileApp.Models
{
    using System;

    public class CancelEvent
    {
        public Guid EventId { get; set; }

        public string CancellationReason { get; set; }
    }
}
