namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents event-partner location service data for display purposes.
    /// </summary>
    public class DisplayEventPartnerLocationService
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
        /// Gets or sets a value indicating whether advance notice is required for this service.
        /// </summary>
        public bool IsAdvanceNoticeRequired { get; set; }

        /// <summary>
        /// Gets or sets public notes about the partner location service.
        /// </summary>
        public string PartnerLocationServicePublicNotes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner.
        /// </summary>
        public string PartnerName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the name of the partner location.
        /// </summary>
        public string PartnerLocationName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status identifier for the event-partner location service.
        /// </summary>
        public int EventPartnerLocationServiceStatusId { get; set; }
    }
}