using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class EventType
    {
        public EventType()
        {
            Events = new HashSet<Event>();
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool? IsActive { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
