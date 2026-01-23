#nullable disable

namespace TrashMob.Models
{
    /// <summary>
    /// Represents a location where collected trash can be picked up after an event.
    /// </summary>
    public class PickupLocation : KeyedModel
    {
        /// <summary>
        /// Gets or sets the identifier of the associated event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets the street address of the pickup location.
        /// </summary>
        public string StreetAddress { get; set; } = "";

        /// <summary>
        /// Gets or sets the city of the pickup location.
        /// </summary>
        public string City { get; set; } = "";

        /// <summary>
        /// Gets or sets the region or state of the pickup location.
        /// </summary>
        public string Region { get; set; } = "";

        /// <summary>
        /// Gets or sets the postal code of the pickup location.
        /// </summary>
        public string PostalCode { get; set; } = "";

        /// <summary>
        /// Gets or sets the country of the pickup location.
        /// </summary>
        public string Country { get; set; } = "";

        /// <summary>
        /// Gets or sets the county of the pickup location.
        /// </summary>
        public string County { get; set; } = "";

        /// <summary>
        /// Gets or sets the latitude coordinate of the pickup location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude coordinate of the pickup location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the pickup location has been submitted for pickup.
        /// </summary>
        public bool HasBeenSubmitted { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the trash at this location has been picked up.
        /// </summary>
        public bool HasBeenPickedUp { get; set; } = false;

        /// <summary>
        /// Gets or sets any notes about the pickup location.
        /// </summary>
        public string Notes { get; set; } = "";

        /// <summary>
        /// Gets or sets the name of the pickup location.
        /// </summary>
        public string Name { get; set; } = "";

        /// <summary>
        /// Gets or sets the event associated with this pickup location.
        /// </summary>
        public virtual Event Event { get; set; }
    }
}