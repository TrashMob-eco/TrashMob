#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerRequest : KeyedModel
    {
        public PartnerRequest()
        {
        }

        public string Name { get; set; }

        public string PrimaryEmail { get; set; }
        
        public string SecondaryEmail { get; set; }

        public string PrimaryPhone { get; set; }

        public string SecondaryPhone { get; set; }

        public string Notes { get; set; }

        public int PartnerRequestStatusId { get; set; }

        public virtual PartnerRequestStatus PartnerRequestStatus { get; set; }
    }
}
