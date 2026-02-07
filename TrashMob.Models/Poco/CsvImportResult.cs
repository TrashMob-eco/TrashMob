namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of a CSV import operation.
    /// </summary>
    public class CsvImportResult
    {
        /// <summary>Gets or sets the number of prospects created.</summary>
        public int CreatedCount { get; set; }

        /// <summary>Gets or sets the number of rows skipped as duplicates.</summary>
        public int SkippedDuplicateCount { get; set; }

        /// <summary>Gets or sets the number of rows that had errors.</summary>
        public int ErrorCount { get; set; }

        /// <summary>Gets or sets the per-row error details.</summary>
        public List<CsvImportError> Errors { get; set; } = [];

        /// <summary>Gets or sets the total number of data rows processed.</summary>
        public int TotalRowsProcessed { get; set; }
    }

    /// <summary>
    /// An error encountered while importing a single CSV row.
    /// </summary>
    public class CsvImportError
    {
        /// <summary>Gets or sets the 1-based row number.</summary>
        public int RowNumber { get; set; }

        /// <summary>Gets or sets the error message.</summary>
        public string? Message { get; set; }
    }
}
