#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of an event (e.g., Active, Full, Canceled, Complete).
    /// </summary>
    /// <remarks>
    /// Event status is inferred from today's date relative to the event start time. Upcoming events
    /// are Active. Events past their start time may be Complete (lead may no longer be at the meetup
    /// location). Canceled events are hidden from public view. Completed events cannot be deleted
    /// for legal tracking purposes.
    /// </remarks>
    public class EventStatus : LookupModel
    {
        /// <summary>
        /// Gets or sets the collection of events with this status.
        /// </summary>
        public virtual ICollection<Event> Events { get; set; } = [];
    }
}