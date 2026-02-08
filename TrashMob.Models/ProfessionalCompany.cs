#nullable disable

namespace TrashMob.Models
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Represents a professional cleanup company that services sponsored adoptable areas.
    /// </summary>
    public class ProfessionalCompany : KeyedModel
    {
        public ProfessionalCompany()
        {
            CompanyUsers = [];
            SponsoredAdoptions = [];
            CleanupLogs = [];
        }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the company's contact email.
        /// </summary>
        public string ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the company's contact phone number.
        /// </summary>
        public string ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this company is assigned to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets whether this company is active.
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Gets or sets the partner (community) navigation property.
        /// </summary>
        public virtual Partner Partner { get; set; }

        /// <summary>
        /// Gets or sets the collection of users associated with this company.
        /// </summary>
        public virtual ICollection<ProfessionalCompanyUser> CompanyUsers { get; set; }

        /// <summary>
        /// Gets or sets the collection of sponsored adoptions assigned to this company.
        /// </summary>
        public virtual ICollection<SponsoredAdoption> SponsoredAdoptions { get; set; }

        /// <summary>
        /// Gets or sets the collection of cleanup logs recorded by this company.
        /// </summary>
        public virtual ICollection<ProfessionalCleanupLog> CleanupLogs { get; set; }
    }
}
