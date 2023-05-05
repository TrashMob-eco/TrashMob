
namespace TrashMob.Models.Poco
{
    using System;

    public class DisplayEventPartnerLocation
    {
        public Guid EventId { get; set; }

        public Guid PartnerId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public string PartnerLocationNotes { get; set; } = string.Empty;

        public string PartnerName { get; set; } = string.Empty;

        public string PartnerLocationName { get; set; } = string.Empty;

        public int EventPartnerStatusId { get; set; }

        public string PartnerServicesEngaged { get; set; } = string.Empty;
    }
}
