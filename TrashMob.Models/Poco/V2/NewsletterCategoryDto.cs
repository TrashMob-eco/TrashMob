#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a newsletter category.
/// </summary>
public class NewsletterCategoryDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public int Id { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the description.</summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>Gets or sets whether new users are auto-subscribed.</summary>
    public bool IsDefault { get; set; }
}
