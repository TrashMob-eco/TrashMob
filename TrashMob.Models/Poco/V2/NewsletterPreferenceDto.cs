#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a user's newsletter preference for a category.
/// </summary>
public class NewsletterPreferenceDto
{
    /// <summary>Gets or sets the category identifier.</summary>
    public int CategoryId { get; set; }

    /// <summary>Gets or sets the category name.</summary>
    public string CategoryName { get; set; } = string.Empty;

    /// <summary>Gets or sets the category description.</summary>
    public string CategoryDescription { get; set; } = string.Empty;

    /// <summary>Gets or sets whether subscribed.</summary>
    public bool IsSubscribed { get; set; }

    /// <summary>Gets or sets the subscribed date.</summary>
    public DateTimeOffset? SubscribedDate { get; set; }

    /// <summary>Gets or sets the unsubscribed date.</summary>
    public DateTimeOffset? UnsubscribedDate { get; set; }
}
