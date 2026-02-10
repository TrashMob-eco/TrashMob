namespace TrashMob.Models.Poco
{
    using System.Collections.Generic;

    /// <summary>
    /// Result of parsing an uploaded area import file.
    /// </summary>
    public class AreaImportParseResult
    {
        /// <summary>
        /// Gets or sets the parsed features.
        /// </summary>
        public List<AreaImportFeature> Features { get; set; } = new();

        /// <summary>
        /// Gets or sets all unique property keys found across all features.
        /// </summary>
        public List<string> PropertyKeys { get; set; } = new();

        /// <summary>
        /// Gets or sets the total number of features found in the file.
        /// </summary>
        public int TotalFeatures { get; set; }

        /// <summary>
        /// Gets or sets the number of features with valid geometry.
        /// </summary>
        public int ValidFeatures { get; set; }

        /// <summary>
        /// Gets or sets any non-fatal warnings encountered during parsing.
        /// </summary>
        public List<string> Warnings { get; set; } = new();

        /// <summary>
        /// Gets or sets a fatal parse error message, if any.
        /// </summary>
        public string? Error { get; set; }
    }
}
