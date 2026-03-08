#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Query parameters for filtering and paginating partners in V2 API.
    /// </summary>
    public class PartnerQueryParameters : QueryParameters
    {
        /// <summary>
        /// Gets or sets an optional partner type filter (Government=1, Business=2, Community=3).
        /// </summary>
        public int? PartnerTypeId { get; set; }

        /// <summary>
        /// Gets or sets an optional city filter.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets an optional region (state/province) filter.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets an optional country filter.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets an optional filter for partners with an enabled community home page.
        /// </summary>
        public bool? HomePageEnabled { get; set; }

        /// <summary>
        /// Gets or sets an optional filter for featured partners.
        /// </summary>
        public bool? IsFeatured { get; set; }
    }
}
