#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class PartnerStatus
    {
        public PartnerStatus()
        {
            Partners = new HashSet<Partner>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }

        public virtual ICollection<Partner> Partners { get; set; }
    }
}
