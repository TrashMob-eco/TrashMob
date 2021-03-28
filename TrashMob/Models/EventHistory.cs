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
        public string CreatedByUserId { get; set; }
        public DateTimeOffset? CreatedDate { get; set; }
        public string LastUpdatedByUserId { get; set; }
        public DateTimeOffset? LastUpdatedDate { get; set; }

        public EventHistory(Event originalEvent)
        {
            Id = originalEvent.Id;
            Name = originalEvent.Name;
            Description = originalEvent.Description;
            EventDate = originalEvent.EventDate;
            EventTypeId = originalEvent.EventTypeId;
            EventStatusId = originalEvent.EventStatusId.Value;
            StreetAddress = originalEvent.StreetAddress;
            City = originalEvent.City;
            StateProvince = originalEvent.StateProvince;
            Country = originalEvent.Country;
            ZipCode = originalEvent.ZipCode;
            Latitude = originalEvent.Latitude;
            Longitude = originalEvent.Longitude;
            Gpscoords = originalEvent.Gpscoords;
            MaxNumberOfParticipants = originalEvent.MaxNumberOfParticipants;
            ActualNumberOfParticipants = originalEvent.ActualNumberOfParticipants;
            CreatedByUserId = originalEvent.CreatedByUserId;
            CreatedDate = originalEvent.CreatedDate;
            LastUpdatedByUserId = originalEvent.LastUpdatedByUserId;
            LastUpdatedDate = originalEvent.LastUpdatedDate;
        }
    }
}
