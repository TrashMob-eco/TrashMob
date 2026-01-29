#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a cleanup event organized through TrashMob.
    /// </summary>
    /// <remarks>
    /// An event is an organized effort to clean up litter at a specific location at a specific date and time.
    /// The user who creates the event is the event lead. Events can be public (visible to all users and included
    /// in notifications) or private (for individual tracking without notifying others). Private events can be
    /// backdated to record past individual cleanup efforts. Completed events cannot be deleted for legal
    /// tracking purposes. Attendees must sign the TrashMob.eco Liability Waiver before registering for their
    /// first event.
    /// </remarks>
    public class Event : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Event"/> class.
        /// </summary>
        public Event()
        {
            UserNotifications = [];
            PickupLocations = [];
            EventAttendees = [];
        }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the event.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the event is scheduled to occur.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the duration of the event in hours.
        /// </summary>
        public int DurationHours { get; set; }

        /// <summary>
        /// Gets or sets the duration of the event in minutes (in addition to hours).
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event type.
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event status.
        /// </summary>
        public int EventStatusId { get; set; }

        /// <summary>
        /// Gets or sets the street address where the event takes place.
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the city where the event takes place.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state where the event takes place.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country where the event takes place.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the event location.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the event location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the event location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of participants allowed for the event.
        /// </summary>
        public int MaxNumberOfParticipants { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event is publicly visible.
        /// </summary>
        public bool IsEventPublic { get; set; }

        /// <summary>
        /// Gets or sets the reason for event cancellation, if applicable.
        /// </summary>
        public string CancellationReason { get; set; }

        /// <summary>
        /// Gets or sets the status of the event.
        /// </summary>
        public virtual EventStatus EventStatus { get; set; }

        /// <summary>
        /// Gets or sets the type of the event.
        /// </summary>
        public virtual EventType EventType { get; set; }

        /// <summary>
        /// Gets or sets the summary information for the event after completion.
        /// </summary>
        public virtual EventSummary EventSummary { get; set; }

        /// <summary>
        /// Gets or sets the collection of attendees registered for the event.
        /// </summary>
        public virtual ICollection<EventAttendee> EventAttendees { get; set; }

        /// <summary>
        /// Gets or sets the collection of routes taken by attendees during the event.
        /// </summary>
        public virtual ICollection<EventAttendeeRoute> EventAttendeeRoutes { get; set; }

        /// <summary>
        /// Gets or sets the collection of user notifications associated with the event.
        /// </summary>
        public virtual ICollection<UserNotification> UserNotifications { get; set; }

        /// <summary>
        /// Gets or sets the collection of pickup locations for the event.
        /// </summary>
        public virtual ICollection<PickupLocation> PickupLocations { get; set; }

        /// <summary>
        /// Gets or sets the collection of litter reports associated with the event.
        /// </summary>
        public virtual ICollection<EventLitterReport> EventLitterReports { get; set; }
    }
}