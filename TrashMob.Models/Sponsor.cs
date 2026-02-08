#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a sponsor who pays for an adoptable area to be maintained by a professional company.
    /// </summary>
    public class Sponsor : KeyedModel
    {
        public Sponsor()
        {
            SponsoredAdoptions = [];
        }

        /// <summary>
        /// Gets or sets the sponsor's name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the sponsor's contact email.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the sponsor's contact phone number.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the URL to the sponsor's logo image.
        /// </summary>
        public string LogoUrl { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this sponsor belongs to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets whether this sponsor is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets whether this sponsor's name should be displayed on the public community map.
        /// </summary>
        public bool ShowOnPublicMap { get; set; } = true;

        /// <summary>
        /// Gets or sets the partner (community) navigation property.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the collection of sponsored adoptions for this sponsor.
        /// </summary>
        public virtual ICollection<SponsoredAdoption> SponsoredAdoptions { get; set; }
    }
}
