#nullable enable

namespace TrashMob.Shared.Extensions
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using TrashMob.Models.Poco.V2;

    /// <summary>
    /// Extension methods for IQueryable to support V2 API pagination.
    /// </summary>
    public static class QueryableExtensions
    {
        /// <summary>
        /// Applies pagination to an IQueryable and maps entities to DTOs, returning a PagedResponse.
        /// </summary>
        /// <typeparam name="TEntity">The entity type in the queryable.</typeparam>
        /// <typeparam name="TDto">The DTO type to map to.</typeparam>
        /// <param name="query">The IQueryable source.</param>
        /// <param name="parameters">The pagination parameters.</param>
        /// <param name="mapper">A function to map each entity to a DTO.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paged response containing the mapped DTOs and pagination metadata.</returns>
        public static async Task<PagedResponse<TDto>> ToPagedAsync<TEntity, TDto>(
            this IQueryable<TEntity> query,
            QueryParameters parameters,
            Func<TEntity, TDto> mapper,
            CancellationToken cancellationToken = default)
        {
            var page = Math.Max(1, parameters.Page);
            var pageSize = parameters.PageSize;

            var totalCount = await query.CountAsync(cancellationToken);
            var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResponse<TDto>
            {
                Items = items.Select(mapper).ToList(),
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = totalPages,
                },
            };
        }

        /// <summary>
        /// Wraps a pre-computed list of items with pagination metadata.
        /// </summary>
        /// <typeparam name="T">The item type.</typeparam>
        /// <param name="items">The items for the current page.</param>
        /// <param name="totalCount">The total count of all items.</param>
        /// <param name="page">The current page number.</param>
        /// <param name="pageSize">The page size.</param>
        /// <returns>A paged response with pagination metadata.</returns>
        public static PagedResponse<T> ToPagedResponse<T>(
            this IReadOnlyList<T> items,
            int totalCount,
            int page,
            int pageSize)
        {
            return new PagedResponse<T>
            {
                Items = items,
                Pagination = new PaginationMetadata
                {
                    Page = page,
                    PageSize = pageSize,
                    TotalCount = totalCount,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                },
            };
        }
    }
}
