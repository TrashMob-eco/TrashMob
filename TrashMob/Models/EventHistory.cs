using System;
using System.Collections.Generic;

#nullable disable

namespace TrashMob.Models
{
    public partial class EventHistory
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTimeOffset EventDate { get; set; }
        public Guid EventTypeId { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string StateProvince { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CreatedByUserId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string Gpscoords { get; set; }
        public int? MaxNumberOfParticipants { get; set; }
        public string LastUpdatedByUserId { get; set; }
        public DateTimeOffset? LastUpdatedDate { get; set; }
    }
}
