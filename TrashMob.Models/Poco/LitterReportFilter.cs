namespace TrashMob.Models.Poco
{
    /// <summary>
    /// Represents filter criteria for querying litter reports.
    /// </summary>
    public class LitterReportFilter : GeneralFilter
    {
        /// <summary>
        /// Gets or sets the optional litter report status identifier to filter by.
        /// </summary>
        public int? LitterReportStatusId { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include associated litter images in the results.
        /// </summary>
        public bool IncludeLitterImages { get; set; } = false;
    }
}