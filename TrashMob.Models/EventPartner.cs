#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class EventPartner : BaseModel
    {
        public EventPartner()
        {
        }

        public Guid EventId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int EventPartnerStatusId { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual Event Event { get; set; }

        public virtual EventPartnerStatus EventPartnerStatus { get; set; }
    }
}
