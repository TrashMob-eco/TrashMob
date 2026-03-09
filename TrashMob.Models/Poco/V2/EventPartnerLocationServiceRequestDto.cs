#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for creating or updating an event partner location service.
/// </summary>
public class EventPartnerLocationServiceRequestDto
{
    /// <summary>Gets or sets the event identifier.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets the partner location identifier.</summary>
    public Guid PartnerLocationId { get; set; }

    /// <summary>Gets or sets the service type identifier.</summary>
    public int ServiceTypeId { get; set; }

    /// <summary>Gets or sets the service status identifier.</summary>
    public int EventPartnerLocationServiceStatusId { get; set; }
}
