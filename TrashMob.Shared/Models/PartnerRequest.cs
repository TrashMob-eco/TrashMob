#nullable disable

namespace TrashMob.Shared.Models
{
    using System;

    public partial class PartnerRequest
    {
        public PartnerRequest()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string PrimaryEmail { get; set; }
        
        public string SecondaryEmail { get; set; }

        public string PrimaryPhone { get; set; }

        public string SecondaryPhone { get; set; }

        public string Notes { get; set; }

        public int PartnerRequestStatusId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual PartnerRequestStatus PartnerRequestStatus { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
