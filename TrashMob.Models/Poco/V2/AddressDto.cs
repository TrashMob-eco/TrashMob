#nullable enable

namespace TrashMob.Models.Poco.V2;

/// <summary>
/// Represents a physical address returned from geocoding.
/// </summary>
public class AddressDto
{
    /// <summary>
    /// Gets or sets the street address.
    /// </summary>
    public string StreetAddress { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the city name.
    /// </summary>
    public string City { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the region or state.
    /// </summary>
    public string Region { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the postal or ZIP code.
    /// </summary>
    public string PostalCode { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the country name.
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the county name.
    /// </summary>
    public string County { get; set; } = string.Empty;
}
