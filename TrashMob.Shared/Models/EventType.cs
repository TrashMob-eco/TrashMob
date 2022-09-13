#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class EventType : LookupModel
    {
        public EventType()
        {
            Events = new HashSet<Event>();
        }

        public virtual ICollection<Event> Events { get; set; }
    }
}
