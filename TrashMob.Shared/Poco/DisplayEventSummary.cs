
namespace TrashMob.Poco
{
    using System;

    public class DisplayEventSummary
    {
        public Guid EventId { get; set; }

        public string Name { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public int EventTypeId { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double NumberOfBags { get; set; }

        public int DurationInMinutes { get; set; }

        public int ActualNumberOfAttendees { get; set; }

        public double TotalWorkHours { get; set; }
    }
}
