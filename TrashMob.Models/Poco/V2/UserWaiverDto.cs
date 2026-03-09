#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a user's waiver acceptance record.
/// </summary>
public class UserWaiverDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the user identifier.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the waiver version identifier.</summary>
    public Guid WaiverVersionId { get; set; }

    /// <summary>Gets or sets when the waiver was accepted.</summary>
    public DateTimeOffset AcceptedDate { get; set; }

    /// <summary>Gets or sets when this acceptance expires.</summary>
    public DateTimeOffset ExpiryDate { get; set; }

    /// <summary>Gets or sets the typed legal name.</summary>
    public string TypedLegalName { get; set; } = string.Empty;

    /// <summary>Gets or sets the signing method.</summary>
    public string SigningMethod { get; set; } = string.Empty;

    /// <summary>Gets or sets the document URL.</summary>
    public string DocumentUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets whether the signer was a minor.</summary>
    public bool IsMinor { get; set; }

    /// <summary>Gets or sets the guardian name.</summary>
    public string GuardianName { get; set; } = string.Empty;

    /// <summary>Gets or sets the guardian relationship.</summary>
    public string GuardianRelationship { get; set; } = string.Empty;
}
