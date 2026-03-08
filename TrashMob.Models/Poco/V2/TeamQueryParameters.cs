#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Query parameters for filtering and paginating teams in V2 API.
    /// </summary>
    public class TeamQueryParameters : QueryParameters
    {
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
    }
}
