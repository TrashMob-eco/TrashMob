namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents event-partner location relationship data for display purposes.
    /// </summary>
    public class DisplayEventPartnerLocation
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner location.
        /// </summary>
        public Guid PartnerLocationId { get; set; }

        /// <summary>
        /// Gets or sets notes specific to the partner location.
        /// </summary>
        public string PartnerLocationNotes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner.
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner location.
        /// </summary>
        public string PartnerLocationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status identifier for the event-partner relationship.
        /// </summary>
        public int EventPartnerStatusId { get; set; }

        /// <summary>
        /// Gets or sets a description of the partner services engaged for this event.
        /// </summary>
        public string PartnerServicesEngaged { get; set; } = string.Empty;
    }
}