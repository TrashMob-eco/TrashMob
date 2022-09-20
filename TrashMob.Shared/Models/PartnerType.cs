#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class PartnerType : LookupModel
    {
        public PartnerType()
        {
            Partners = new HashSet<Partner>();
        }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}
