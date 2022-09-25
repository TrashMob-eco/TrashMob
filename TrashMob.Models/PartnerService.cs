#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public partial class PartnerService : BaseModel
    {
        public PartnerService()
        {
        }

        public Guid PartnerId { get; set; }

        public int ServiceTypeId { get; set; }

        public string Notes { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual ServiceType ServiceType { get; set; }
    }
}
