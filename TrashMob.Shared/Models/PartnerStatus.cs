#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class PartnerStatus : LookupModel
    {
        public PartnerStatus()
        {
            Partners = new HashSet<Partner>();
        }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}
