#nullable disable

namespace TrashMob.Models
{
    public class PartnerRequestStatus : LookupModel
    {
        public PartnerRequestStatus()
        {
            PartnerRequests = new HashSet<PartnerRequest>();
        }

        public virtual ICollection<PartnerRequest> PartnerRequests { get; set; }
    }
}