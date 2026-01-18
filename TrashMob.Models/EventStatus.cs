#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the status of an event (e.g., Active, Full, Canceled, Complete).
    /// </summary>
    public class EventStatus : LookupModel
    {
        /// <summary>
        /// Gets or sets the collection of events with this status.
        /// </summary>
        public virtual ICollection<Event> Events { get; set; } = [];
    }
}