#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerContact : KeyedModel
    {
        public Guid PartnerId { get; set; }

        public int PartnerContactTypeId { get; set; }

        public string Name { get; set; }
        
        public string Email { get; set; }
        
        public string Phone { get; set; }

        public string Notes { get; set; }

        public bool? IsActive { get; set; }

        public virtual Partner Partner { get; set; }
    }
}
