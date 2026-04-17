#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for child-initiated consent (Flow 3).
/// </summary>
public class InitiateChildConsentRequest
{
    /// <summary>Gets or sets the child's first name.</summary>
    public string ChildFirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the child's email address.</summary>
    public string ChildEmail { get; set; } = string.Empty;

    /// <summary>Gets or sets the child's date of birth.</summary>
    public DateOnly ChildBirthDate { get; set; }

    /// <summary>Gets or sets the parent's email address.</summary>
    public string ParentEmail { get; set; } = string.Empty;
}
