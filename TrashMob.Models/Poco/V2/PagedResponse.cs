#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// A paginated response wrapper for API v2 endpoints.
    /// </summary>
    /// <typeparam name="T">The type of items in the response.</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Gets or sets the items for the current page.
        /// </summary>
        public IReadOnlyList<T> Items { get; set; } = [];

        /// <summary>
        /// Gets or sets the pagination metadata.
        /// </summary>
        public PaginationMetadata Pagination { get; set; } = new();
    }

    /// <summary>
    /// Metadata describing the pagination state of a response.
    /// </summary>
    public class PaginationMetadata
    {
        /// <summary>
        /// Gets or sets the current page number (1-based).
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Gets or sets the number of items per page.
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Gets or sets the total number of items across all pages.
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        /// Gets or sets the total number of pages.
        /// </summary>
        public int TotalPages { get; set; }

        /// <summary>
        /// Gets a value indicating whether there is a next page.
        /// </summary>
        public bool HasNext => Page < TotalPages;

        /// <summary>
        /// Gets a value indicating whether there is a previous page.
        /// </summary>
        public bool HasPrevious => Page > 1;
    }
}
