#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Request body for creating a new Instant Event — a zero-friction solo private cleanup
/// where the server auto-fills title, description, date, event type, visibility, and status.
/// See Planning/Projects/Project_65_Instant_Events.md for context.
/// </summary>
public class InstantEventRequestDto
{
    /// <summary>Gets or sets the latitude of the user's current location at Start. Required.</summary>
    public double Latitude { get; set; }

    /// <summary>Gets or sets the longitude of the user's current location at Start. Required.</summary>
    public double Longitude { get; set; }
}
