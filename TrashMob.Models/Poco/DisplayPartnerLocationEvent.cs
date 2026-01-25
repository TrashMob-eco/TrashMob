namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents an event associated with a partner location service for display purposes.
    /// </summary>
    public class DisplayPartnerLocationServiceEvent
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner location.
        /// </summary>
        public Guid PartnerLocationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the service type.
        /// </summary>
        public int ServiceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the date and time of the event.
        /// </summary>
        public DateTimeOffset EventDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the event.
        /// </summary>
        public string EventName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the street address of the event.
        /// </summary>
        public string EventStreetAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the city of the event.
        /// </summary>
        public string EventCity { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region or state of the event.
        /// </summary>
        public string EventRegion { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country of the event.
        /// </summary>
        public string EventCountry { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code of the event.
        /// </summary>
        public string EventPostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the event.
        /// </summary>
        public string EventDescription { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner.
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner location.
        /// </summary>
        public string PartnerLocationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status identifier for the event-partner location relationship.
        /// </summary>
        public int EventPartnerLocationStatusId { get; set; }
    }
}