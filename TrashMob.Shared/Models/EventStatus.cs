#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class EventStatus : LookupModel
    {
        public EventStatus()
        {
            Events = new HashSet<Event>();
        }

        public virtual ICollection<Event> Events { get; set; }
    }
}
