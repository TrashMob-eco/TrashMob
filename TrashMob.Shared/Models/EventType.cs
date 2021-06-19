#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class EventType
    {
        public EventType()
        {
            Events = new HashSet<Event>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }
        
        public bool? IsActive { get; set; }

        public virtual ICollection<Event> Events { get; set; }
    }
}
