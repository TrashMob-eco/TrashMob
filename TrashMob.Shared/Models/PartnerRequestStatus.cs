#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class PartnerRequestStatus
    {
        public PartnerRequestStatus()
        {
            PartnerRequests = new HashSet<PartnerRequest>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }

        public virtual ICollection<PartnerRequest> PartnerRequests { get; set; }
    }
}
