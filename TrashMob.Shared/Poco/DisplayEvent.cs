namespace TrashMob.Shared.Poco
{
    using System;

    /// <summary>
    /// Represents a read-only view of an event for display purposes.
    /// </summary>
    public class DisplayEvent
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        public Guid Id { get; set; }

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
        /// Gets or sets the hours portion of the event duration.
        /// </summary>
        public int DurationHours { get; set; }

        /// <summary>
        /// Gets or sets the minutes portion of the event duration.
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
        public int? MaxNumberOfParticipants { get; set; }

        /// <summary>
        /// Gets or sets the visibility level of the event (Public=1, TeamOnly=2, Private=3).
        /// </summary>
        public int EventVisibilityId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the team this event is scoped to, when visibility is TeamOnly.
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who created the event.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the event was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the user who last updated the event.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the event was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the username of the user who created the event.
        /// </summary>
        public string CreatedByUserName { get; set; }
    }
}