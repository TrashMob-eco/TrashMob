#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// A paginated response wrapper that properly serializes pagination metadata.
/// Unlike PaginatedList (which extends List and loses metadata during serialization),
/// this DTO exposes Items, PageIndex, TotalPages as top-level properties.
/// </summary>
/// <typeparam name="T">The type of items in the response.</typeparam>
public class PaginatedResponseDto<T>
{
    /// <summary>Gets or sets the items for the current page.</summary>
    public IReadOnlyList<T> Items { get; set; } = [];

    /// <summary>Gets or sets the current page index (1-based).</summary>
    public int PageIndex { get; set; }

    /// <summary>Gets or sets the total number of pages.</summary>
    public int TotalPages { get; set; }

    /// <summary>Gets a value indicating whether there is a previous page.</summary>
    public bool HasPreviousPage => PageIndex > 1;

    /// <summary>Gets a value indicating whether there is a next page.</summary>
    public bool HasNextPage => PageIndex < TotalPages;
}
