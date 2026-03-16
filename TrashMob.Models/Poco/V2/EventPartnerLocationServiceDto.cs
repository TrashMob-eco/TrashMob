#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// V2 API response representation of an event partner location service.
/// </summary>
public class EventPartnerLocationServiceDto
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
