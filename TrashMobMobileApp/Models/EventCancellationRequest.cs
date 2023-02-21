namespace TrashMobMobileApp.Models
{
    using System;

    public class EventCancellationRequest
    {
        public Guid EventId { get; set; }

        public string CancellationReason { get; set; }
    }
}
