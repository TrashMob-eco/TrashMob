#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a partner organization that supports TrashMob events.
    /// </summary>
    /// <remarks>
    /// Partners provide services to event leads and attendees to help with event logistics.
    /// A partner may be an individual, government agency, private business, or other organization.
    /// Partners can have multiple locations, each offering different services: disposal locations (dumpsters),
    /// hauling (pickup of bagged garbage after events), supplies (food, drinks, garbage bags), or
    /// equipment (buckets, reflective vests, litter pickers, gloves). Partners provide social media
    /// contact info to help amplify events.
    /// </remarks>
    public class Partner : KeyedModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Partner"/> class.
        /// </summary>
        public Partner()
        {
            PartnerContacts = new HashSet<PartnerContact>();
            PartnerDocuments = new HashSet<PartnerDocument>();
            PartnerSocialMediaAccounts = new HashSet<PartnerSocialMediaAccount>();
            PartnerLocations = new HashSet<PartnerLocation>();
            PartnerAdmins = new HashSet<PartnerAdmin>();
            PartnerAdminInvitations = new HashSet<PartnerAdminInvitation>();
            AdoptableAreas = new HashSet<AdoptableArea>();
        }

        /// <summary>
        /// Gets or sets the name of the partner organization.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner status.
        /// </summary>
        public int PartnerStatusId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the partner type.
        /// </summary>
        public int PartnerTypeId { get; set; }

        /// <summary>
        /// Gets or sets the website URL of the partner organization.
        /// </summary>
        public string Website { get; set; }

        /// <summary>
        /// Gets or sets publicly visible notes about the partner.
        /// </summary>
        public string PublicNotes { get; set; }

        /// <summary>
        /// Gets or sets private notes about the partner (visible only to admins).
        /// </summary>
        public string PrivateNotes { get; set; }

        #region Community Home Page Properties

        /// <summary>
        /// Gets or sets the URL-friendly slug for the community page (e.g., "seattle-wa").
        /// </summary>
        public string Slug { get; set; }

        /// <summary>
        /// Gets or sets whether the community home page is enabled.
        /// </summary>
        public bool HomePageEnabled { get; set; }

        /// <summary>
        /// Gets or sets when the community home page becomes active.
        /// </summary>
        public DateTimeOffset? HomePageStartDate { get; set; }

        /// <summary>
        /// Gets or sets when the community home page expires.
        /// </summary>
        public DateTimeOffset? HomePageEndDate { get; set; }

        /// <summary>
        /// Gets or sets the primary branding color (hex format, e.g., "#3B82F6").
        /// </summary>
        public string BrandingPrimaryColor { get; set; }

        /// <summary>
        /// Gets or sets the secondary branding color (hex format, e.g., "#1E40AF").
        /// </summary>
        public string BrandingSecondaryColor { get; set; }

        /// <summary>
        /// Gets or sets the URL of the community banner image.
        /// </summary>
        public string BannerImageUrl { get; set; }

        /// <summary>
        /// Gets or sets the tagline displayed on the community page.
        /// </summary>
        public string Tagline { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the community center point.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the community center point.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the city for the community.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        /// Gets or sets the region/state for the community.
        /// </summary>
        public string Region { get; set; }

        /// <summary>
        /// Gets or sets the country for the community.
        /// </summary>
        public string Country { get; set; }

        /// <summary>
        /// Gets or sets the URL of the community logo (200x200).
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the contact email for the community.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the contact phone for the community.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the physical address for display.
        /// </summary>
        public string PhysicalAddress { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the collection of contacts for the partner.
        /// </summary>
        public virtual ICollection<PartnerContact> PartnerContacts { get; set; }

        /// <summary>
        /// Gets or sets the collection of documents associated with the partner.
        /// </summary>
        public virtual ICollection<PartnerDocument> PartnerDocuments { get; set; }

        /// <summary>
        /// Gets or sets the collection of locations for the partner.
        /// </summary>
        public virtual ICollection<PartnerLocation> PartnerLocations { get; set; }

        /// <summary>
        /// Gets or sets the collection of social media accounts for the partner.
        /// </summary>
        public virtual ICollection<PartnerSocialMediaAccount> PartnerSocialMediaAccounts { get; set; }

        /// <summary>
        /// Gets or sets the collection of administrators for the partner.
        /// </summary>
        public virtual ICollection<PartnerAdmin> PartnerAdmins { get; set; }

        /// <summary>
        /// Gets or sets the collection of admin invitations for the partner.
        /// </summary>
        public virtual ICollection<PartnerAdminInvitation> PartnerAdminInvitations { get; set; }

        /// <summary>
        /// Gets or sets the collection of adoptable areas for the community.
        /// </summary>
        public virtual ICollection<AdoptableArea> AdoptableAreas { get; set; }

        /// <summary>
        /// Gets or sets the status of the partner.
        /// </summary>
        public virtual PartnerStatus PartnerStatus { get; set; }

        /// <summary>
        /// Gets or sets the type of the partner.
        /// </summary>
        public virtual PartnerType PartnerType { get; set; }
    }
}