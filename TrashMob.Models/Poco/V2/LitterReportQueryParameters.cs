#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// Query parameters for filtering and paginating litter reports in V2 API.
    /// </summary>
    public class LitterReportQueryParameters : QueryParameters
    {
        /// <summary>
        /// Gets or sets an optional litter report status filter (New=1, Assigned=2, Cleaned=3, Cancelled=4).
        /// </summary>
        public int? LitterReportStatusId { get; set; }

        /// <summary>
        /// Gets or sets an optional city filter (matches against litter image locations).
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
        /// Gets or sets the start of the date range filter.
        /// </summary>
        public DateTimeOffset? FromDate { get; set; }

        /// <summary>
        /// Gets or sets the end of the date range filter.
        /// </summary>
        public DateTimeOffset? ToDate { get; set; }

        /// <summary>
        /// Gets or sets an optional filter for reports created by a specific user.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }
    }
}
