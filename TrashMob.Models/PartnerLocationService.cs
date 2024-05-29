#nullable disable

namespace TrashMob.Models
{
    public class PartnerLocationService : BaseModel
    {
        public Guid PartnerLocationId { get; set; }

        public int ServiceTypeId { get; set; }

        public bool IsAutoApproved { get; set; } = false;

        public bool IsAdvanceNoticeRequired { get; set; } = true;

        public string Notes { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }
}