namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents general filter criteria for querying data with pagination support.
    /// </summary>
    public class GeneralFilter
    {
        /// <summary>
        /// Gets or sets the optional start date to filter results from.
        /// </summary>
        public DateTimeOffset? StartDate { get; set; }

        /// <summary>
        /// Gets or sets the optional end date to filter results until.
        /// </summary>
        public DateTimeOffset? EndDate { get; set; }

        /// <summary>
        /// Gets or sets the optional country to filter by.
        /// </summary>
        public string? Country { get; set; }

        /// <summary>
        /// Gets or sets the optional region or state to filter by.
        /// </summary>
        public string? Region { get; set; }

        /// <summary>
        /// Gets or sets the optional city to filter by.
        /// </summary>
        public string? City { get; set; }

        /// <summary>
        /// Gets or sets the optional user identifier to filter by creator.
        /// </summary>
        public Guid? CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the optional page size for pagination.
        /// </summary>
        public int? PageSize { get; set; }

        /// <summary>
        /// Gets or sets the optional page index for pagination.
        /// </summary>
        public int? PageIndex { get; set; }
    }
}