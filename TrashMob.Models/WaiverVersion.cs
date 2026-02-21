#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a versioned waiver document with effective dates.
    /// </summary>
    public class WaiverVersion : KeyedModel
    {
        /// <summary>
        /// Gets or sets the waiver name (e.g., "TrashMob", "Seattle Parks").
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the version string (e.g., "1.0", "2.0").
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets the full waiver text content (HTML supported).
        /// </summary>
        public string WaiverText { get; set; }

        /// <summary>
        /// Gets or sets when this waiver version becomes effective.
        /// </summary>
        public DateTimeOffset EffectiveDate { get; set; }

        /// <summary>
        /// Gets or sets when this waiver version expires/is superseded (null = current version).
        /// </summary>
        public DateTimeOffset? ExpiryDate { get; set; }

        /// <summary>
        /// Gets or sets whether this waiver version is currently active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the scope of the waiver (Global or Community).
        /// </summary>
        public WaiverScope Scope { get; set; }

        /// <summary>
        /// Gets or sets the collection of community assignments for this waiver.
        /// </summary>
        public virtual ICollection<CommunityWaiver> CommunityWaivers { get; set; }

        /// <summary>
        /// Gets or sets the collection of user acceptances of this waiver.
        /// </summary>
        public virtual ICollection<UserWaiver> UserWaivers { get; set; }
    }
}
