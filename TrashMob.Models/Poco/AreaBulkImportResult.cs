namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of a bulk area import operation.
    /// </summary>
    public class AreaBulkImportResult
    {
        /// <summary>
        /// Gets or sets the number of areas successfully created.
        /// </summary>
        public int CreatedCount { get; set; }

        /// <summary>
        /// Gets or sets the number of areas skipped due to duplicate names.
        /// </summary>
        public int SkippedDuplicateCount { get; set; }

        /// <summary>
        /// Gets or sets the number of areas that failed to import.
        /// </summary>
        public int ErrorCount { get; set; }

        /// <summary>
        /// Gets or sets the detailed error list.
        /// </summary>
        public List<AreaImportError> Errors { get; set; } = new();

        /// <summary>
        /// Gets or sets the total number of areas processed.
        /// </summary>
        public int TotalProcessed { get; set; }
    }

    /// <summary>
    /// Represents a single error during bulk area import.
    /// </summary>
    public class AreaImportError
    {
        /// <summary>
        /// Gets or sets the zero-based index of the feature that failed.
        /// </summary>
        public int FeatureIndex { get; set; }

        /// <summary>
        /// Gets or sets the name of the feature that failed.
        /// </summary>
        public string FeatureName { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the error message.
        /// </summary>
        public string Message { get; set; } = string.Empty;
    }
}
