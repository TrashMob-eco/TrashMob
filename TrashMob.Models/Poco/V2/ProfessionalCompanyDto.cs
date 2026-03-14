#nullable enable

namespace TrashMob.Models.Poco.V2
{
    using System;

    /// <summary>
    /// V2 API representation of a professional cleanup company. Flat DTO excluding navigation properties.
    /// </summary>
    public class ProfessionalCompanyDto
    {
        /// <summary>
        /// Gets or sets the unique identifier.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the company name.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the company's contact email.
        /// </summary>
        public string? ContactEmail { get; set; }

        /// <summary>
        /// Gets or sets the company's contact phone number.
        /// </summary>
        public string? ContactPhone { get; set; }

        /// <summary>
        /// Gets or sets the community (partner) this company is assigned to.
        /// </summary>
        public Guid PartnerId { get; set; }

        /// <summary>
        /// Gets or sets whether this company is active.
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Gets or sets when the company was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }
    }
}
