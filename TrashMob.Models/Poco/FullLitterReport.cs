namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents a litter report with full details including associated images for display purposes.
    /// </summary>
    public class FullLitterReport
    {
        /// <summary>
        /// Gets or sets the unique identifier of the litter report.
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Gets or sets the name or title of the litter report.
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the description of the litter report.
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the status identifier of the litter report.
        /// </summary>
        public int LitterReportStatusId { get; set; } = 1;

        /// <summary>
        /// Gets or sets the collection of images associated with this litter report.
        /// </summary>
        public List<FullLitterImage> LitterImages { get; set; } = [];

        /// <summary>
        /// Gets or sets the identifier of the user who created the report.
        /// </summary>
        public Guid CreatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the report was created.
        /// </summary>
        public DateTimeOffset? CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the user who last updated the report.
        /// </summary>
        public Guid LastUpdatedByUserId { get; set; }

        /// <summary>
        /// Gets or sets the date and time when the report was last updated.
        /// </summary>
        public DateTimeOffset? LastUpdatedDate { get; set; }

        /// <summary>
        /// Gets or sets the name of the user who created the report.
        /// </summary>
        public string CreateByUserName { get; set; } = string.Empty;
    }
}