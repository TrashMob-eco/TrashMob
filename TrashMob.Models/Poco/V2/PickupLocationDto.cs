#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a pickup location for collected trash.
/// </summary>
public class PickupLocationDto
{
    /// <summary>Gets or sets the identifier.</summary>
    public Guid Id { get; set; }

    /// <summary>Gets or sets the event identifier.</summary>
    public Guid EventId { get; set; }

    /// <summary>Gets or sets the name.</summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>Gets or sets the street address.</summary>
    public string StreetAddress { get; set; } = string.Empty;

    /// <summary>Gets or sets the city.</summary>
    public string City { get; set; } = string.Empty;

    /// <summary>Gets or sets the region or state.</summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>Gets or sets the postal code.</summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>Gets or sets the country.</summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>Gets or sets the county.</summary>
    public string County { get; set; } = string.Empty;

    /// <summary>Gets or sets the latitude.</summary>
    public double? Latitude { get; set; }

    /// <summary>Gets or sets the longitude.</summary>
    public double? Longitude { get; set; }

    /// <summary>Gets or sets whether locations have been submitted for pickup.</summary>
    public bool HasBeenSubmitted { get; set; }

    /// <summary>Gets or sets whether the trash has been picked up.</summary>
    public bool HasBeenPickedUp { get; set; }

    /// <summary>Gets or sets any notes.</summary>
    public string Notes { get; set; } = string.Empty;
}
