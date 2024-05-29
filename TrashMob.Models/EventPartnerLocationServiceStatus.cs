#nullable disable

namespace TrashMob.Models
{
    public class EventPartnerLocationServiceStatus : LookupModel
    {
        public EventPartnerLocationServiceStatus()
        {
            EventPartnerLocationServices = new HashSet<EventPartnerLocationService>();
        }

        public virtual ICollection<EventPartnerLocationService> EventPartnerLocationServices { get; set; }
    }
}