#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    public partial class Event : KeyedModel
    {
        public Event()
        {
            UserNotifications = new HashSet<UserNotification>();
            PickupLocations = new HashSet<PickupLocation>();
            EventAttendees = new HashSet<EventAttendee>();
        }

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

        public string CancellationReason { get; set; }

        public virtual EventStatus EventStatus { get; set; }

        public virtual EventType EventType { get; set; }

        public virtual EventSummary EventSummary { get; set; }

        public virtual ICollection<EventAttendee> EventAttendees {get;set;}

        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        public virtual ICollection<PickupLocation> PickupLocations { get; set; }
    }
}
