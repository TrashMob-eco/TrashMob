#nullable disable

using TrashMob;

namespace TrashMob.Models
{
    using System;

    public partial class EventPartner
    {
        public EventPartner()
        {
        }

        public Guid EventId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int EventPartnerStatusId { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }

        public virtual Partner Partner { get; set; }

        public virtual PartnerLocation PartnerLocation { get; set; }

        public virtual Event Event { get; set; }

        public virtual EventPartnerStatus EventPartnerStatus { get; set; }
    }
}
