#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a contact request submission.
/// </summary>
public class ContactRequestDto
{
    /// <summary>
    /// Gets or sets the name of the person making the contact request.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the message content.
    /// </summary>
    public string Message { get; set; } = string.Empty;
}
