
namespace TrashMob.Shared.Poco
{
    using System;

    public class DisplayEventPartner
    {
        public Guid EventId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public string PartnerLocationNotes { get; set; }

        public string PartnerName { get; set; }

        public string PartnerLocationName { get; set; }

        public int EventPartnerStatusId { get; set; }
    }
}
