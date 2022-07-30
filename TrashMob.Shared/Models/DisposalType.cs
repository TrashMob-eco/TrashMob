#nullable disable

namespace TrashMob.Shared.Models
{
    using System.Collections.Generic;

    public partial class DisposalType
    {
        public DisposalType()
        {
            Communities = new HashSet<Community>();
        }

        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Description { get; set; }
        
        public int? DisplayOrder { get; set; }
        
        public bool? IsActive { get; set; }

        public virtual ICollection<Community> Communities { get; set; }
    }
}
