#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a physical location of a partner organization.
    /// </summary>
    public class PartnerLocation : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PartnerLocation"/> class.
        /// </summary>
        public PartnerLocation()
        {
            PartnerLocationContacts = new HashSet<PartnerLocationContact>();
            PartnerLocationServices = new HashSet<PartnerLocationService>();
        }

        /// <summary>
        /// Gets or sets the identifier of the partner organization.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets the name of the location.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the street address of the location.
        /// </summary>
        public string StreetAddress { get; set; }

        /// <summary>
        /// Gets or sets the city of the location.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region or state of the location.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country of the location.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the postal code of the location.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        /// Gets or sets the latitude coordinate of the location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets publicly visible notes about the location.
        /// </summary>
        public string PublicNotes { get; set; }

        /// <summary>
        /// Gets or sets private notes about the location (visible only to admins).
        /// </summary>
        public string PrivateNotes { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the location is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the partner organization this location belongs to.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the collection of contacts for this location.
        /// </summary>
        public virtual ICollection<PartnerLocationContact> PartnerLocationContacts { get; set; }

        /// <summary>
        /// Gets or sets the collection of services available at this location.
        /// </summary>
        public virtual ICollection<PartnerLocationService> PartnerLocationServices { get; set; }
    }
}