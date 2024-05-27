#nullable disable

namespace TrashMob.Models
{
    public class EventStatus : LookupModel
    {
        public EventStatus()
        {
            Events = new HashSet<Event>();
        }

        public virtual ICollection<Event> Events { get; set; }
    }
}