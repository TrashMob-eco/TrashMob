#nullable disable

using TrashMob;

namespace TrashMob.Models
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
