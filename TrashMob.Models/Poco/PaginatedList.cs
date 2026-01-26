namespace TrashMob.Models.Poco
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Represents a paginated list of items with navigation properties.
    /// </summary>
    /// <typeparam name="T">The type of items in the list.</typeparam>
    public class PaginatedList<T> : List<T>
    {
        /// <summary>
        /// Gets the current page index (1-based).
        /// </summary>
        public int PageIndex { get; private set; }

        /// <summary>
        /// Gets the total number of pages available.
        /// </summary>
        public int TotalPages { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class.
        /// </summary>
        public PaginatedList()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginatedList{T}"/> class with the specified items and pagination info.
        /// </summary>
        /// <param name="items">The list of items for the current page.</param>
        /// <param name="count">The total count of all items across all pages.</param>
        /// <param name="pageIndex">The current page index (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        /// <summary>
        /// Gets a value indicating whether there is a previous page available.
        /// </summary>
        public bool HasPreviousPage => PageIndex > 1;

        /// <summary>
        /// Gets a value indicating whether there is a next page available.
        /// </summary>
        public bool HasNextPage => PageIndex < TotalPages;

        /// <summary>
        /// Creates a paginated list from a queryable source.
        /// </summary>
        /// <param name="source">The queryable source to paginate.</param>
        /// <param name="pageIndex">The page index to retrieve (1-based).</param>
        /// <param name="pageSize">The number of items per page.</param>
        /// <returns>A paginated list containing items for the specified page.</returns>
        public static PaginatedList<T> Create(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
