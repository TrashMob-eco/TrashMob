
namespace TrashMob.Models.Poco
{
    using System;

    public class DisplayPartnerLocationServiceEvent
    {
        public Guid EventId { get; set; }

        public Guid PartnerLocationId { get; set; }

        public int ServiceTypeId { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public string EventName { get; set; } = string.Empty;

        public string EventStreetAddress { get; set; } = string.Empty;

        public string EventCity { get; set; } = string.Empty;

        public string EventRegion { get; set; } = string.Empty;

        public string EventCountry { get; set; } = string.Empty;

        public string EventPostalCode { get; set; } = string.Empty;

        public string EventDescription { get; set; } = string.Empty;

        public string PartnerName { get; set; } = string.Empty;

        public string PartnerLocationName { get; set; } = string.Empty;

        public int EventPartnerLocationStatusId { get; set; }
    }
}
