#nullable disable

namespace TrashMob.Models
{
    using System;

    /// <summary>
    /// Associates a waiver version with a community (partner).
    /// </summary>
    public class CommunityWaiver : KeyedModel
    {
        /// <summary>
        /// Gets or sets the community (partner) identifier.
        /// </summary>
        public Guid CommunityId { get; set; }

        /// <summary>
        /// Gets or sets the waiver version identifier.
        /// </summary>
        public Guid WaiverVersionId { get; set; }

        /// <summary>
        /// Gets or sets whether this waiver is required for the community.
        /// </summary>
        public bool IsRequired { get; set; } = true;

        /// <summary>
        /// Gets or sets the community (partner) navigation property.
        /// </summary>
        public virtual Partner Community { get; set; }

        /// <summary>
        /// Gets or sets the waiver version navigation property.
        /// </summary>
        public virtual WaiverVersion WaiverVersion { get; set; }
    }
}
