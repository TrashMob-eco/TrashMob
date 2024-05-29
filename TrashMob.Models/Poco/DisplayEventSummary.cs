namespace TrashMob.Models.Poco
{
    public class DisplayEventSummary
    {
        public Guid EventId { get; set; }

        public string Name { get; set; } = string.Empty;
        public DateTimeOffset EventDate { get; set; }

        public int EventTypeId { get; set; }

        public string StreetAddress { get; set; } = string.Empty;

        public string City { get; set; } = string.Empty;

        public string Region { get; set; } = string.Empty;

        public string Country { get; set; } = string.Empty;

        public string PostalCode { get; set; } = string.Empty;

        public double NumberOfBags { get; set; }

        public int DurationInMinutes { get; set; }

        public int ActualNumberOfAttendees { get; set; }

        public double TotalWorkHours { get; set; }
    }
}