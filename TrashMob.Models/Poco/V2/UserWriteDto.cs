#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API request DTO for creating or updating a user profile. Includes writable PII fields
    /// that are excluded from the read-only <see cref="UserDto"/>.
    /// </summary>
    public class UserWriteDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the user.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the display username.
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's email address.
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's given (first) name.
        /// </summary>
        public string GivenName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's surname (last name).
        /// </summary>
        public string Surname { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's city.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's region (state/province).
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's country.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the user's postal code.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the latitude of the user's location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the user's location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets whether the user prefers metric units.
        /// </summary>
        public bool PrefersMetric { get; set; }

        /// <summary>
        /// Gets or sets the user's date of birth (used for age verification).
        /// </summary>
        public DateTimeOffset? DateOfBirth { get; set; }

        /// <summary>
        /// Gets or sets whether the user is a minor (13-17).
        /// </summary>
        public bool IsMinor { get; set; }

        /// <summary>
        /// Gets or sets the maximum travel distance for local event notifications.
        /// </summary>
        public int TravelLimitForLocalEvents { get; set; }

        /// <summary>
        /// Gets or sets the URL of the user's profile photo.
        /// </summary>
        public string ProfilePhotoUrl { get; set; } = string.Empty;
    }
}
