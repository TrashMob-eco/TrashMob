namespace TrashMobMobile.Models
{
    public class EventCancellationRequest
    {
        public Guid EventId { get; set; }

        public string CancellationReason { get; set; }
    }
}