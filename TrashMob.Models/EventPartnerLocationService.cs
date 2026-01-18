#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a service provided by a partner location for an event.
    /// </summary>
    public class EventPartnerLocationService : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner location providing the service.
        /// </summary>
        public Guid PartnerLocationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the type of service being provided.
        /// </summary>
        public int ServiceTypeId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the current status of this service request.
        /// </summary>
        public int EventPartnerLocationServiceStatusId { get; set; }

        /// <summary>
        /// Gets or sets the partner location providing the service.
        /// </summary>
        public virtual PartnerLocation PartnerLocation { get; set; }

        /// <summary>
        /// Gets or sets the event receiving the service.
        /// </summary>
        public virtual Event Event { get; set; }

        /// <summary>
        /// Gets or sets the type of service being provided.
        /// </summary>
        public virtual ServiceType ServiceType { get; set; }

        /// <summary>
        /// Gets or sets the current status of the service request.
        /// </summary>
        public virtual EventPartnerLocationServiceStatus EventPartnerLocationServiceStatus { get; set; }
    }
}