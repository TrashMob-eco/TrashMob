#nullable disable

namespace TrashMob.Models
{
    using System;

    public partial class EventPartnerLocation : BaseModel
    {
        public EventPartnerLocation()
        {
        }

        public Guid EventId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int EventPartnerLocationStatusId { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual Event Event { get; set; }

        public virtual EventPartnerLocationStatus EventPartnerLocationStatus { get; set; }
    }
}
