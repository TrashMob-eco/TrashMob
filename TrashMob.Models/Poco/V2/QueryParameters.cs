#nullable enable

namespace TrashMob.Models.Poco.V2
{
    /// <summary>
    /// Base query parameters for paginated API endpoints.
    /// </summary>
    public class QueryParameters
    {
        private const int MaxPageSize = 100;
        private const int DefaultPageSize = 25;

        private int pageSize = DefaultPageSize;

        /// <summary>
        /// Gets or sets the page number (1-based). Defaults to 1.
        /// </summary>
        public int Page { get; set; } = 1;

        /// <summary>
        /// Gets or sets the number of items per page. Maximum is 100, default is 25.
        /// </summary>
        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
        }

        /// <summary>
        /// Gets or sets the sort field and direction (e.g., "name", "date desc").
        /// </summary>
        public string? Sort { get; set; }
    }
}
