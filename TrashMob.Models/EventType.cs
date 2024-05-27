#nullable disable

namespace TrashMob.Models
{
    public class EventType : LookupModel
    {
        public EventType()
        {
            Events = new HashSet<Event>();
        }

        public virtual ICollection<Event> Events { get; set; }
    }
}