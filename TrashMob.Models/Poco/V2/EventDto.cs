#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of an event. Flat DTO with no navigation properties.
    /// </summary>
    public class EventDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the event.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the event name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the event description.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the duration in hours.
        /// </summary>
        public int DurationHours { get; set; }

        /// <summary>
        /// Gets or sets the duration in minutes.
        /// </summary>
        public int DurationMinutes { get; set; }

        /// <summary>
        /// Gets or sets the event type identifier.
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Gets or sets the event status identifier.
        /// </summary>
        public int EventStatusId { get; set; }

        /// <summary>
        /// Gets or sets the street address.
        /// </summary>
        public string StreetAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region (state/province).
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the maximum number of participants.
        /// </summary>
        public int MaxNumberOfParticipants { get; set; }

        /// <summary>
        /// Gets or sets whether the event is publicly visible.
        /// </summary>
        public bool IsEventPublic { get; set; }

        /// <summary>
        /// Gets or sets the event visibility identifier (maps to EventVisibilityEnum).
        /// </summary>
        public int EventVisibilityId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the event.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the event was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the event.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the event was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the optional team identifier associated with this event.
        /// </summary>
        public Guid? TeamId { get; set; }

        /// <summary>
        /// Gets or sets the username of the event creator.
        /// </summary>
        public string CreatedByUserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the current user is attending this event (contextual, may be null).
        /// </summary>
        public bool? IsAttending { get; set; }
    }
}
