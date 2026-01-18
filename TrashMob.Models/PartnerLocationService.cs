#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a service offered by a partner at a specific location.
    /// </summary>
    public class PartnerLocationService : BaseModel
    {
        /// <summary>
        /// Gets or sets the identifier of the partner location.
        /// </summary>
        public Guid PartnerLocationId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the service type.
        /// </summary>
        public int ServiceTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether service requests are automatically approved.
        /// </summary>
        public bool IsAutoApproved { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether advance notice is required for this service.
        /// </summary>
        public bool IsAdvanceNoticeRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets any notes about the service.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the partner location offering this service.
        /// </summary>
        public virtual PartnerLocation PartnerLocation { get; set; }

        /// <summary>
        /// Gets or sets the type of service being offered.
        /// </summary>
        public virtual ServiceType ServiceType { get; set; }
    }
}