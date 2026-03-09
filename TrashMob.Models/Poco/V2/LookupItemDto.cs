#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a lookup/reference data item (e.g., event type, service type).
/// </summary>
public class LookupItemDto
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// Gets or sets the name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the description.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display order for sorting.
    /// </summary>
    public int DisplayOrder { get; set; }
}
