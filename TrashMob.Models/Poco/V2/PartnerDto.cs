#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a partner. Excludes admin-only fields (PrivateNotes, contact info, adoptable area defaults).
    /// </summary>
    public class PartnerDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the partner.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the partner organization.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the website URL.
        /// </summary>
        public string Website { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets publicly visible notes about the partner.
        /// </summary>
        public string PublicNotes { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the partner's logo.
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the partner status identifier (Active=1, Inactive=2).
        /// </summary>
        public int PartnerStatusId { get; set; }

        /// <summary>
        /// Gets or sets the partner type identifier (Government=1, Business=2, Community=3).
        /// </summary>
        public int PartnerTypeId { get; set; }

        /// <summary>
        /// Gets or sets the URL-friendly slug for the community page.
        /// </summary>
        public string Slug { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the community home page is enabled.
        /// </summary>
        public bool HomePageEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether this community is featured.
        /// </summary>
        public bool IsFeatured { get; set; }

        /// <summary>
        /// Gets or sets the primary branding color (hex format).
        /// </summary>
        public string BrandingPrimaryColor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the secondary branding color (hex format).
        /// </summary>
        public string BrandingSecondaryColor { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the community banner image.
        /// </summary>
        public string BannerImageUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the tagline displayed on the community page.
        /// </summary>
        public string Tagline { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude of the partner's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the partner's location.
        /// </summary>
        public double? Longitude { get; set; }

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
        /// Gets or sets the physical address for display.
        /// </summary>
        public string PhysicalAddress { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the northern latitude bound for bounding-box filtering.
        /// </summary>
        public double? BoundsNorth { get; set; }

        /// <summary>
        /// Gets or sets the southern latitude bound.
        /// </summary>
        public double? BoundsSouth { get; set; }

        /// <summary>
        /// Gets or sets the eastern longitude bound.
        /// </summary>
        public double? BoundsEast { get; set; }

        /// <summary>
        /// Gets or sets the western longitude bound.
        /// </summary>
        public double? BoundsWest { get; set; }

        /// <summary>
        /// Gets or sets the GeoJSON polygon representing the community boundary.
        /// </summary>
        public string BoundaryGeoJson { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the type of geographic region (City, County, State, etc.).
        /// </summary>
        public int? RegionType { get; set; }

        /// <summary>
        /// Gets or sets the county name for county-level communities.
        /// </summary>
        public string CountyName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the identifier of the user who created the partner.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the partner was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the partner was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
