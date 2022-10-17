#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class EventPartnerLocationStatus : LookupModel
    {
        public EventPartnerLocationStatus()
        {
            EventPartnerLocations = new HashSet<EventPartnerLocation>();
        }

        public virtual ICollection<EventPartnerLocation> EventPartnerLocations { get; set; }
    }
}
