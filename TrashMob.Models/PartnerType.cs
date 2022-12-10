#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System.Collections.Generic;

    public partial class PartnerType : LookupModel
    {
        public PartnerType()
        {
            PartnerRequests = new HashSet<PartnerRequest>();
            Partners = new HashSet<Partner>();
        }

        public virtual ICollection<Partner> Partners { get; set; }

        public virtual ICollection<PartnerRequest> PartnerRequests { get; set; }
    }
}
