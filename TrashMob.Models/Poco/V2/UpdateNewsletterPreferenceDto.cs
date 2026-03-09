#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for updating a newsletter preference.
/// </summary>
public class UpdateNewsletterPreferenceDto
{
    /// <summary>Gets or sets the newsletter category identifier.</summary>
    public int CategoryId { get; set; }

    /// <summary>Gets or sets whether subscribed.</summary>
    public bool IsSubscribed { get; set; }
}
