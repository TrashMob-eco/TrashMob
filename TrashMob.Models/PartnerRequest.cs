#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a request to become a partner organization.
    /// </summary>
    public class PartnerRequest : KeyedModel
    {
        /// <summary>
        /// Gets or sets the name of the organization requesting partnership.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the email address for the partnership request.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the website URL of the requesting organization.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets the phone number for the partnership request.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        /// Gets or sets the city of the requesting organization.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state of the requesting organization.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country of the requesting organization.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the requesting organization.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the organization's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the organization's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets any notes about the partnership request.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the request status.
        /// </summary>
        public int PartnerRequestStatusId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner type.
        /// </summary>
        public int PartnerTypeId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this is a "Become a Partner" request.
        /// </summary>
        public bool isBecomeAPartnerRequest { get; set; }

        /// <summary>
        /// Gets or sets the status of the partnership request.
        /// </summary>
        public virtual PartnerRequestStatus PartnerRequestStatus { get; set; }

        /// <summary>
        /// Gets or sets the type of partner being requested.
        /// </summary>
        public virtual PartnerType PartnerType { get; set; }
    }
}