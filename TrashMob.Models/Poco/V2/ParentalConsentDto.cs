#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// V2 API representation of a PRIVO parental consent record.
/// </summary>
public class ParentalConsentDto
{
    /// <summary>Gets or sets the consent record identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the user this consent applies to.</summary>
    public Guid UserId { get; set; }

    /// <summary>Gets or sets the parent user identifier (for child consent flows).</summary>
    public Guid? ParentUserId { get; set; }

    /// <summary>Gets or sets the dependent identifier (for parent-initiated flows).</summary>
    public Guid? DependentId { get; set; }

    /// <summary>Gets or sets the consent type (1=AdultVerification, 2=ParentInitiatedChild, 3=ChildInitiated).</summary>
    public int ConsentType { get; set; }

    /// <summary>Gets or sets the consent status (1=Pending, 2=Verified, 3=Denied, 4=Expired, 5=Revoked).</summary>
    public int Status { get; set; }

    /// <summary>Gets or sets the consent URL for redirecting to PRIVO.</summary>
    public string ConsentUrl { get; set; } = string.Empty;

    /// <summary>Gets or sets when consent was verified.</summary>
    public DateTimeOffset? VerifiedDate { get; set; }

    /// <summary>Gets or sets when the consent was created.</summary>
    public DateTimeOffset? CreatedDate { get; set; }
}
