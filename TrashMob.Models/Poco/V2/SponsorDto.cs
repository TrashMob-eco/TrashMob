#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a sponsor. Flat DTO excluding navigation properties.
    /// </summary>
    public class SponsorDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the sponsor's name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the sponsor's contact email.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the sponsor's contact phone number.
        /// </summary>
        public string? ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the URL to the sponsor's logo image.
        /// </summary>
        public string? LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this sponsor belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets whether this sponsor is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets whether this sponsor's name should be displayed on the public community map.
        /// </summary>
        public bool ShowOnPublicMap { get; set; }

        /// <summary>
        /// Gets or sets when the sponsor was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
