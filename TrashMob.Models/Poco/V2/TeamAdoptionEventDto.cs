#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a team adoption event link. Flat DTO excluding navigation properties.
    /// </summary>
    public class TeamAdoptionEventDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the team adoption.
        /// </summary>
        public Guid TeamAdoptionId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the cleanup event.
        /// </summary>
        public Guid EventId { get; set; }

        /// <summary>
        /// Gets or sets optional notes about why this event is linked to the adoption.
        /// </summary>
        public string? Notes { get; set; }

        /// <summary>
        /// Gets or sets when the link was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
