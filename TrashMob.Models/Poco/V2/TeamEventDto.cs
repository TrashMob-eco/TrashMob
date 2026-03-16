#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// V2 API representation of a team-event association.
/// </summary>
public class TeamEventDto
{
    /// <summary>Gets or sets the unique identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the team identifier.</summary>
    public Guid TeamId { get; set; }

    /// <summary>Gets or sets the event identifier.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets when this association was created.</summary>
    public DateTimeOffset? CreatedDate { get; set; }
}
