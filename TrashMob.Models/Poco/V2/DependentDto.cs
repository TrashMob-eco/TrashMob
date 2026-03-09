#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a dependent (minor) linked to a parent account.
/// </summary>
public class DependentDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the parent user identifier.</summary>
    public Guid ParentUserId { get; set; }

    /// <summary>Gets or sets the first name.</summary>
    public string FirstName { get; set; } = string.Empty;

    /// <summary>Gets or sets the last name.</summary>
    public string LastName { get; set; } = string.Empty;

    /// <summary>Gets or sets the date of birth.</summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>Gets or sets the relationship (parent, legal guardian, etc.).</summary>
    public string Relationship { get; set; } = string.Empty;

    /// <summary>Gets or sets optional medical notes.</summary>
    public string MedicalNotes { get; set; } = string.Empty;

    /// <summary>Gets or sets the emergency contact phone.</summary>
    public string EmergencyContactPhone { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the dependent is active.</summary>
    public bool IsActive { get; set; }
}
