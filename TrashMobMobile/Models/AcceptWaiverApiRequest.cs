namespace TrashMobMobile.Models;

public class AcceptWaiverApiRequest
{
    public Guid WaiverVersionId { get; set; }

    public string TypedLegalName { get; set; } = string.Empty;

    public bool IsMinor { get; set; }

    public Guid? GuardianUserId { get; set; }

    public string? GuardianName { get; set; }

    public string? GuardianRelationship { get; set; }
}
