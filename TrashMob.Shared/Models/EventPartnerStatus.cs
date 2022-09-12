#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class EventPartnerStatus : LookupModel
    {
        public EventPartnerStatus()
        {
            EventPartners = new HashSet<EventPartner>();
        }

        public virtual ICollection<EventPartner> EventPartners { get; set; }
    }
}
