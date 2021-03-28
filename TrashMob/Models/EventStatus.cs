#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class EventStatus
    {
        public EventStatus()
        {
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
