#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Event
    {
        public Event()
        {
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public DateTimeOffset EventDate { get; set; }

        public int EventTypeId { get; set; }

        public int EventStatusId { get; set; }

        public string StreetAddress { get; set; }

        public string City { get; set; }

        public string StateProvince { get; set; }

        public string Country { get; set; }

        public string ZipCode { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public string Gpscoords { get; set; }

        public int? MaxNumberOfParticipants { get; set; }

        public int? ActualNumberOfParticipants { get; set; }

        public Guid CreatedByUserId { get; set; }

        public DateTimeOffset? CreatedDate { get; set; }

        public Guid LastUpdatedByUserId { get; set; }

        public DateTimeOffset? LastUpdatedDate { get; set; }

        public virtual EventStatus EventStatus { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual User CreatedByUser { get; set; }

        public virtual User LastUpdatedByUser { get; set; }
    }
}
