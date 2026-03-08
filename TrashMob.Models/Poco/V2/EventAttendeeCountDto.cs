#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an event's active attendee count.
    /// </summary>
    public class EventAttendeeCountDto
    {
        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the number of active attendees.
        /// </summary>
        public int Count { get; set; }
    }
}
