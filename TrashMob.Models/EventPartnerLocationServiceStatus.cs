#nullable disable

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class EventPartnerLocationServiceStatus : LookupModel
    {
        public EventPartnerLocationServiceStatus()
        {
            EventPartnerLocationServices = new HashSet<EventPartnerLocationService>();
        }

        public virtual ICollection<EventPartnerLocationService> EventPartnerLocationServices { get; set; }
    }
}
