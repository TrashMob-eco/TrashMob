#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class EventPartnerStatus
    {
        public EventPartnerStatus()
        {
            EventPartners = new HashSet<EventPartner>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }

        public virtual ICollection<EventPartner> EventPartners { get; set; }
    }
}
