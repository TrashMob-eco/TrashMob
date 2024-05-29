namespace TrashMob.Shared.Poco
{
    using System;

    public class DisplayEvent
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public int DurationHours { get; set; }

        public int DurationMinutes { get; set; }

        public int EventTypeId { get; set; }

        public int EventStatusId { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string Region { get; set; }

        public string Country { get; set; }

        public string PostalCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public int? MaxNumberOfParticipants { get; set; }

        public bool IsEventPublic { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public string CreatedByUserName { get; set; }
    }
}