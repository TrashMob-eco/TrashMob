#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents the type or category of an event.
    /// </summary>
    public class EventType : LookupModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventType"/> class.
        /// </summary>
        public EventType()
        {
            Events = new HashSet<Event>();
        }

        /// <summary>
        /// Gets or sets the collection of events of this type.
        /// </summary>
        public virtual ICollection<Event> Events { get; set; }
    }
}