namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a summary view of an event with key metrics for display purposes.
    /// </summary>
    public class DisplayEventSummary
    {
        /// <summary>
        /// Gets or sets the unique identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the event type.
        /// </summary>
        public int EventTypeId { get; set; }

        /// <summary>
        /// Gets or sets the street address of the event location.
        /// </summary>
        public string StreetAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of the event location.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region or state of the event location.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country of the event location.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code of the event location.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the number of bags of litter collected at the event.
        /// </summary>
        public double NumberOfBags { get; set; }

        /// <summary>
        /// Gets or sets the duration of the event in minutes.
        /// </summary>
        public int DurationInMinutes { get; set; }

        /// <summary>
        /// Gets or sets the actual number of attendees at the event.
        /// </summary>
        public int ActualNumberOfAttendees { get; set; }

        /// <summary>
        /// Gets or sets the total work hours contributed by all attendees.
        /// </summary>
        public double TotalWorkHours { get; set; }
    }
}