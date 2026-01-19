namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents a request to cancel an event.
    /// </summary>
    public class EventCancellationRequest
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event to cancel.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the reason for cancelling the event.
        /// </summary>
        public string CancellationReason { get; set; }
    }
}