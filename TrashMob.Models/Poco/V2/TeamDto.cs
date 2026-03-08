#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a team. Flat DTO excluding navigation properties.
    /// </summary>
    public class TeamDto
    {
        /// <summary>
        /// Gets or sets the unique identifier for the team.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the team.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the team.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the URL of the team's logo image.
        /// </summary>
        public string LogoUrl { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the team is publicly visible.
        /// </summary>
        public bool IsPublic { get; set; }

        /// <summary>
        /// Gets or sets whether joining the team requires approval from a lead.
        /// </summary>
        public bool RequiresApproval { get; set; }

        /// <summary>
        /// Gets or sets the latitude of the team's primary location.
        /// </summary>
        public double? Latitude { get; set; }

        /// <summary>
        /// Gets or sets the longitude of the team's primary location.
        /// </summary>
        public double? Longitude { get; set; }

        /// <summary>
        /// Gets or sets the city where the team is based.
        /// </summary>
        public string City { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the region (state/province) where the team is based.
        /// </summary>
        public string Region { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the country where the team is based.
        /// </summary>
        public string Country { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the postal code of the team's location.
        /// </summary>
        public string PostalCode { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets whether the team is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who created the team.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets when the team was created.
        /// </summary>
        public DateTimeOffset CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets when the team was last updated.
        /// </summary>
        public DateTimeOffset LastUpdatedDate { get; set; }
    }
}
