#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class Partner : KeyedModel
    {
        public Partner()
        {
        }

        public string Name { get; set; }

        public string PrimaryEmail { get; set; }
        
        public string SecondaryEmail { get; set; }

        public string PrimaryPhone { get; set; }

        public string SecondaryPhone { get; set; }

        public string Notes { get; set; }

        public int PartnerStatusId { get; set; }

        public virtual PartnerStatus PartnerStatus { get; set; }
    }
}
